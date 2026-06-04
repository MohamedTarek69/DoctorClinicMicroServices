using System.Security.Claims;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.DTOs.ClinicDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicMicroServices.Presentation.Controllers
{
    [ApiController]
    [Route("doctorclinics")]
    public class ClinicsController : ApiBaseController
    {
        private readonly IClinicService _clinicService;

        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value;
        }

        // =========================
        // Doctor Flow
        // =========================

        [Authorize(Roles = "Doctor")]
        [HttpPost("CreateClinics")]
        public async Task<IActionResult> CreateClinic([FromBody] CreateClinicRequest request)
        {
            var identityUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var result = await _clinicService.CreateClinicAsync(identityUserId, request);
            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("MyClinics")]
        public async Task<IActionResult> GetMyClinics()
        {
            var identityUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var result = await _clinicService.GetMyClinicsAsync(identityUserId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateClinics/{id:int}")]
        public async Task<IActionResult> UpdateClinic(int id, [FromBody] UpdateClinicRequest request)
        {
            var identityUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var result = await _clinicService.UpdateClinicAsync(identityUserId, id, request);
            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpDelete("DeleteClinics/{id:int}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var identityUserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(identityUserId))
                return Unauthorized();

            var result = await _clinicService.DeleteClinicAsync(identityUserId, id);
            return HandleResult(result);
        }

        // =========================
        // Public / Patient Flow
        // =========================

        [AllowAnonymous]
        [HttpGet("GetClinicsById/{id:int}")]
        public async Task<IActionResult> GetClinicById(int id)
        {
            var result = await _clinicService.GetClinicByIdAsync(id);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("GetAllClinics")]
        public async Task<IActionResult> GetAllClinics([FromQuery] ClinicQueryParams qp)
        {
            var result = await _clinicService.GetClinicsAsync(qp);
            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("GetClinicsByDoctorId/{doctorId:guid}")]
        public async Task<IActionResult> GetClinicsByDoctorId(Guid doctorId)
        {
            var result = await _clinicService.GetClinicsByDoctorIdAsync(doctorId);
            return HandleResult(result);
        }
    }
}