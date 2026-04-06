using ClinicMicroServices.Services_Abstraction.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("DoctorClinic/Dashboard/{clinicId}")]
        public async Task<IActionResult> GetClinicDashboard(int clinicId)
        {
            var result = await _dashboardService.GetClinicDashboardAsync(clinicId);

            if (!result.IsSuccess)
                return BadRequest(result.IsFailure);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("Doctor/Dashboard")]
        public async Task<IActionResult> GetDoctorDashboard()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if (!Guid.TryParse(doctorId, out var id))
            //    return Unauthorized();

            var result = await _dashboardService.GetDoctorDashboardAsync(doctorId);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/Dashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();

            if (!result.IsSuccess)
                return BadRequest(result.IsFailure);

            return Ok(result.Value);
        }



    }
}
