using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Persistence.Data.DbContexts;
using ClinicMicroServices.Persistence.Repositories;
using ClinicMicroServices.Services.Services;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared;
using ClinicMicroServices.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ClinicMicroServices.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            #region Add Services to the container
            var builder = WebApplication.CreateBuilder(args);

            var identityAuthority = builder.Configuration["Identity:Authority"];


            builder.Services.AddControllers()
                .AddApplicationPart(typeof(ClinicMicroServices.Presentation.Controllers.ApiBaseController).Assembly);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();

            // ✅ HTTP Client to Identity (for internal calls)
            builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
            {
                client.BaseAddress = new Uri(identityAuthority!);
            });

            // ✅ JWT Authentication (NO Authority because Identity is not OIDC provider)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!)
                    ),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var auth = ctx.Request.Headers["Authorization"].FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(auth) &&
                            auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            var token = auth.Substring("Bearer ".Length).Trim();
                            token = new string(token.Where(c => !char.IsWhiteSpace(c)).ToArray());
                            ctx.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine("JWT Auth Failed: " + ctx.Exception);
                        return Task.CompletedTask;
                    }
                };
            });


            // ✅ Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("DoctorOnly", p => p.RequireRole("Doctor"));
                options.AddPolicy("LabOnly", p => p.RequireRole("Lab"));
                options.AddPolicy("UserOnly", p => p.RequireRole("User"));
                options.AddPolicy("AdminOrDoctor", p => p.RequireRole("Admin", "Doctor"));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5128); // ✅ Clinic port
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new
                    {
                        title = "Validation Error",
                        status = 400,
                        detail = "One or more validation errors occurred",
                        traceId = context.HttpContext.TraceIdentifier,
                        errors = context.ModelState
                            .Where(x => x.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                            )
                    });
                };
            });

            #endregion

            var app = builder.Build();

            #region DataSeed - Apply Migration
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
                await db.Database.MigrateAsync();
            }
            #endregion

            #region Configure the HTTP request pipeline

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseCors("AllowAll");

            app.Use(async (ctx, next) =>
            {
                Console.WriteLine($"REQ: {ctx.Request.Method} {ctx.Request.Path}");
                Console.WriteLine("Authorization Header = " + ctx.Request.Headers["Authorization"].ToString());
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();

            #endregion
        }
    }
}