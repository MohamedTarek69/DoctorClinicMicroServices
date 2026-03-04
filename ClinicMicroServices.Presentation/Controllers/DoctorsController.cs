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

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
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
            if (!User.IsInRole("Admin"))
            {
                var callerUserId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    User.FindFirstValue("sub");

                var isOwner = await _doctorService.IsDoctorOwnerAsync(id, callerUserId!);
                if (!isOwner) return Forbid();
            }

            var result = await _doctorService.UpdateDoctorAsync(id, request);
            return HandleResult(result);
        }

        // ✅ Admin OR Owner Doctor
        [Authorize(Roles = "Admin,Doctor")]
        [HttpPut("UpdateDoctorPassword/{id:guid}")]
        public async Task<IActionResult> UpdatePassword(Guid id, [FromBody] UpdateDoctorPasswordRequest req)
        {
            if (!User.IsInRole("Admin"))
            {
                var callerUserId =
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    User.FindFirstValue("sub");

                var isOwner = await _doctorService.IsDoctorOwnerAsync(id, callerUserId!);
                if (!isOwner) return Forbid();
            }

            var result = await _doctorService.UpdateDoctorPasswordAsync(id, req);
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
    }
}