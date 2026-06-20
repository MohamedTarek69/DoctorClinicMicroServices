using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.DTOs.DoctorDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;


namespace ClinicMicroServices.Presentation.Controllers
{
    [ApiController]
    [Route("doctors")]
    public class DoctorsController : ApiBaseController
    {
        private readonly IDoctorService _doctorService;
        private readonly IIdentityClient _identityClient;

        public DoctorsController(IDoctorService doctorService, IIdentityClient identityClient)
        {
            _doctorService = doctorService;
            _identityClient=identityClient;
        }

        // ✅ Admin creates doctor (create in Identity + store in Clinic DB)
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateDoctor")]
        public async Task<IActionResult> Create([FromBody] CreateDoctorRequest request)
        {
            var result = await _doctorService.CreateDoctorAsync(request);
            return HandleResult(result);
        }

        // ✅ Public list
        [AllowAnonymous]
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAll([FromQuery] ClinicQueryParams qp)
        {
            var result = await _doctorService.GetDoctorsAsync(qp);
            return HandleResult(result);
        }

        // ✅ Public details
        [AllowAnonymous]
        [HttpGet("GetAllDoctorDetatils/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeClinics = false)
        {
            var result = await _doctorService.GetDoctorByIdAsync(id, includeClinics);
            return HandleResult(result);
        }

        // ✅ Admin OR Owner Doctor
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("UpdateDoctor/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorRequest request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            _identityClient.SetToken(token); // 🔥 دي أهم سطر

            if (!User.IsInRole("Admin"))
            {
                var callerUserId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    User.FindFirstValue("sub");

                var isOwner = await _doctorService.IsDoctorOwnerAsync(id, callerUserId!);
                if (!isOwner) return Forbid();
            }

            var result = await _doctorService.UpdateDoctorAsync(id, request, token);
            return HandleResult(result);
        }

        // ✅ Admin OR Owner Doctor
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("UpdateDoctorPassword/{id:guid}")]
        public async Task<IActionResult> UpdatePassword(Guid id, [FromBody] UpdateDoctorPasswordRequest req)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            _identityClient.SetToken(token); // 🔥 نفس القصة

            if (!User.IsInRole("Admin"))
            {
                var callerUserId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    User.FindFirstValue("sub");

                var isOwner = await _doctorService.IsDoctorOwnerAsync(id, callerUserId!);
                if (!isOwner) return Forbid();
            }

            var result = await _doctorService.UpdateDoctorPasswordAsync(id, req,token);
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("deactivate/{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await _doctorService.DeactivateDoctorAsync(id);
            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("activate/{id:guid}")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var result = await _doctorService.ActivateDoctorAsync(id);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("internal/is-active/{identityUserId}")]
        public async Task<IActionResult> IsDoctorActive(string identityUserId)
        {
            var result = await _doctorService.IsDoctorActiveByIdentityUserIdAsync(identityUserId);

            if (result.IsFailure)
                return NotFound(result.Errors);

            return Ok(new { isActive = result.Value });
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("BySpecialty/{specialty}")]
        public async Task<IActionResult> GetBySpecialty(string specialty)
        {
            var doctors = await _doctorService.GetBySpecialty(specialty);

            return Ok(doctors);
        }
    }
}