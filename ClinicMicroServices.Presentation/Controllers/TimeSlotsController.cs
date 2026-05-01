using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared.DTOs.TimeSlotDtos;
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
    [Route("timeslots")]
    public class TimeSlotsController : ApiBaseController
    {
        private readonly ITimeSlotService _service;

        public TimeSlotsController(ITimeSlotService service)
        {
            _service = service;
        }

        // ✅ Create slot (Admin or Doctor)
        [Authorize(Roles = "Doctor")]
        [HttpPost("createtimeslots")]   
        public async Task<IActionResult> Create([FromBody] CreateTimeSlotRequest request)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.CreateAsync(request, doctorId!);
            return HandleResult(result);
        }

        // ✅ All slots for clinic
        [AllowAnonymous]
        [HttpGet("GetTimeSlotsByClinic/{clinicId:int}")]
        public async Task<IActionResult> GetTimeSlotsByClinic(int clinicId)
        {
            var result = await _service.GetByClinicAsync(clinicId);
            return HandleResult(result);
        }

        // ✅ Available slots only
        [AllowAnonymous]
        [HttpGet("getavailabletimeslots/{clinicId:int}")]
        public async Task<IActionResult> GetAvailable(int clinicId)
        {
            var result = await _service.GetAvailableAsync(clinicId);
            return HandleResult(result);
        }

        // ✅ Delete slot
        [Authorize(Roles = "Doctor")]
        [HttpDelete("deletetimeslots/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.DeleteAsync(id, doctorId!);
            return HandleResult(result);
        }

        // ✅ Update slot
        [Authorize(Roles = "Doctor")]
        [HttpPut("updatetimeslots/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTimeSlotRequest request)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.UpdateAsync(id, request, doctorId!);
            return HandleResult(result);
        }
    }
}
