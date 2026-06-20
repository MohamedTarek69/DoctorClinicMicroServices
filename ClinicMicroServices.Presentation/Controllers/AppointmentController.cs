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

        // ✅ Get Appointment Patient Details
        [Authorize(Roles = "AdminOrDoctor")]
        [HttpGet("{appointmentId}/patient")]
        public async Task<IActionResult> GetAppointmentPatientDetails(int appointmentId)
        {
            var token = Request.Headers.Authorization.ToString();

            var result = await _service.GetAppointmentPatientDetailsAsync(appointmentId, token);

            return HandleResult(result);
        }

        // ✅ Get All Appointments (Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _service.GetAllAppointmentsAsync();
            return HandleResult(result);
        }

        #region Clinic Appointments By Status

        [Authorize(Roles = "Doctor")]
        [HttpGet("ShowClinicAppointments")]
        public async Task<IActionResult> ShowClinicAppointments(int clinicId)
        {


            var result = await _service.ShowClinicAppointmentsAsync(clinicId);
            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("clinic/{clinicId:int}/confirmed")]
        public async Task<IActionResult> ShowClinicConfirmedAppointments(int clinicId)
        {
            var result = await _service
                .ShowClinicConfirmedAppointmentsAsync(clinicId);

            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("clinic/{clinicId:int}/pending")]
        public async Task<IActionResult> ShowClinicPendingAppointments(int clinicId)
        {
            var result = await _service
                .ShowClinicPendingAppointmentsAsync(clinicId);

            return HandleResult(result);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("clinic/{clinicId:int}/cancelled")]
        public async Task<IActionResult> ShowClinicCancelledAppointments(int clinicId)
        {
            var result = await _service
                .ShowClinicCancelledAppointmentsAsync(clinicId);

            return HandleResult(result);
        }

        #endregion

        #region Patient Appointments By Status

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

        [Authorize(Roles = "Patient")]
        [HttpGet("patient/confirmed")]
        public async Task<IActionResult> ShowPatientConfirmedAppointments()
        {
            var patientId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            var result = await _service
                .ShowPatientConfirmedAppointmentsAsync(patientId!);

            return HandleResult(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("patient/pending")]
        public async Task<IActionResult> ShowPatientPendingAppointments()
        {
            var patientId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            var result = await _service
                .ShowPatientPendingAppointmentsAsync(patientId!);

            return HandleResult(result);
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("patient/cancelled")]
        public async Task<IActionResult> ShowPatientCancelledAppointments()
        {
            var patientId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            var result = await _service
                .ShowPatientCancelledAppointmentsAsync(patientId!);

            return HandleResult(result);
        }

        #endregion
    }
}
