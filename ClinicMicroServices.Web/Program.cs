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
using System.Text;
using ClinicMicroServices.Web.Factories;
using ClinicMicroServices.Persistence.Data.Config;

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

            // Database
            builder.Services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });

            builder.Services.AddHttpContextAccessor();

            // Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IClinicService, ClinicService>();
            builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            // HTTP Client → Identity Service
            builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
            {
                client.BaseAddress = new Uri(identityAuthority!);
            });

            // JWT Authentication
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
            });

            // Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("DoctorOnly", p => p.RequireRole("Doctor"));
                options.AddPolicy("LabOnly", p => p.RequireRole("Lab"));
                options.AddPolicy("UserOnly", p => p.RequireRole("User"));
                options.AddPolicy("AdminOrDoctor", p => p.RequireRole("Admin", "Doctor"));
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Port
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5128);
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

            #region Configure HTTP Pipeline

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            // Enable CORS
            app.UseCors("AllowAll");

            // Logging middleware
            app.Use(async (ctx, next) =>
            {
                Console.WriteLine($"REQ: {ctx.Request.Method} {ctx.Request.Path}");
                Console.WriteLine("Authorization Header = " + ctx.Request.Headers["Authorization"].ToString());
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion

            await app.RunAsync();
        }
    }
}