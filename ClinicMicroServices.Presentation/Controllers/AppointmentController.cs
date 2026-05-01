using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared.DTOs.Appointment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Route("appointments")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentsController : ApiBaseController
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        // ✅ Book
        [Authorize(Roles = "Patient")]
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] CreateAppointmentRequest request)
        {
            var patientId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            var result = await _service.BookAsync(patientId!, request);

            return HandleResult(result);
        }

        // ✅ Cancel
        [Authorize(Roles = "Patient")]
        [HttpDelete("{appointmentId:int}")]
        public async Task<IActionResult> Cancel(int appointmentId)
        {
            var patientId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            var result = await _service.CancelAsync(appointmentId, patientId!);

            return HandleResult(result);
        }

        // ✅ Update Status (Doctor)
        [Authorize(Roles = "Doctor")]
        [HttpPut("updatestatus")]
        public async Task<IActionResult> UpdateStatus(UpdateAppointmentStatusRequest request)
        {
            var result = await _service.UpdateStatusAsync(request);

            if (result.IsFailure)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("ShowClinicAppointments")]
        public async Task<IActionResult> ShowClinicAppointments(int clinicId)
        {


            var result = await _service.ShowClinicAppointmentsAsync(clinicId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("ShowPatientAppointments")]
        public async Task<IActionResult> ShowPatientAppointments()
        {
            var patientId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");
            var result = await _service.ShowPatientAppointmentsAsync(patientId!);
            return HandleResult(result);
        }
    }
}
