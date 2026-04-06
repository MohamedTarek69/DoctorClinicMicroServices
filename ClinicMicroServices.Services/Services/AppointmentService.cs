using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Services.Specifications.Appointments;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Book

        public async Task<Result<AppointmentResponse>> BookAsync(string patientId, CreateAppointmentRequest request)
        {
            var slotRepo = _unitOfWork.GetRepository<TimeSlot, int>();
            var appointmentRepo = _unitOfWork.GetRepository<Appointment, int>();

            // ✅ Get slot with appointments
            var spec = new TimeSlotAvailableCheckSpec(request.TimeSlotId);
            var list = await slotRepo.GetAllWithSpecAsync(spec);
            var slot = list.FirstOrDefault();

            if (slot is null)
                return Result<AppointmentResponse>.Fail(
                    Error.NotFound("TimeSlot.NotFound", "Time slot not found")
                );

            // ✅ Check capacity
            if (slot.Appointments.Count >= slot.Capacity)
                return Result<AppointmentResponse>.Fail(
                    Error.Validation("TimeSlot.Full", "This slot is fully booked")
                );

            // ✅ Prevent duplicate booking
            var patientGuid = Guid.Parse(patientId);

            var alreadyBooked = slot.Appointments
                .Any(a => a.PatientId == patientGuid);

            if (alreadyBooked)
                return Result<AppointmentResponse>.Fail(
                    Error.Validation("Appointment.Duplicate", "You already booked this slot")
                );

            // ✅ Create appointment
            var appointment = new Appointment
            {
                PatientId = patientGuid,
                ClinicId = slot.ClinicId,
                TimeSlotId = slot.Id,
                Status = AppointmentStatus.Pending,
                PriceAtBooking = slot.Price
            };

            await appointmentRepo.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return Result<AppointmentResponse>.Ok(Map(appointment, slot));
        }

        #endregion

        #region Cancel

        public async Task<Result<bool>> CancelAsync(int appointmentId, string patientId)
        {
            var repo = _unitOfWork.GetRepository<Appointment, int>();

            var appointment = await repo.GetByIdAsync(appointmentId);

            if (appointment is null)
                return Result<bool>.Fail(
                    Error.NotFound("Appointment.NotFound", "Appointment not found")
                );

            var patientGuid = Guid.Parse(patientId);

            // ✅ Ownership check
            if (appointment.PatientId != patientGuid)
                return Result<bool>.Fail(
                    Error.Forbidden("Appointment.Forbidden", "You cannot cancel this appointment")
                );

            // ✅ Already cancelled
            if (appointment.Status == AppointmentStatus.Cancelled)
                return Result<bool>.Fail(
                    Error.Validation("Appointment.Cancelled", "Already cancelled")
                );

            appointment.Status = AppointmentStatus.Cancelled;

            repo.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }

        #endregion

        #region UpdateStatus
        public async Task<Result<AppointmentResponse>> UpdateStatusAsync(UpdateAppointmentStatusRequest request)
        {
            var repo = _unitOfWork.GetRepository<Appointment, int>();


            var appointment = await repo.GetByIdAsync(request.AppointmentId);

            var slotRepo = _unitOfWork.GetRepository<TimeSlot, int>();
            var slot = await slotRepo.GetByIdAsync(appointment!.TimeSlotId);

            if (appointment is null)
                return Result<AppointmentResponse>.Fail(
                    Error.NotFound("Appointment.NotFound", "Appointment not found")
                );

            if (slot is null)
                return Result<AppointmentResponse>.Fail(
                    Error.NotFound("TimeSlot.NotFound", "Time slot not found")
                );

            // ❌ منع تغيير بعد ما يتأكد أو يتلغي
            if (appointment.Status == AppointmentStatus.Cancelled||appointment.Status == AppointmentStatus.Completed)
                return Result<AppointmentResponse>.Fail(
                    Error.Validation("Appointment.InvalidState", "Cannot modify this appointment")
                );


            // ✅ Mapping
            appointment.Status = request.Status switch
            {
                AppointmentStatusDto.Confirmed => AppointmentStatus.Confirmed,
                AppointmentStatusDto.Cancelled => AppointmentStatus.Cancelled,
                _ => AppointmentStatus.Pending
            };

            repo.Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return Result<AppointmentResponse>.Ok(Map(appointment, slot));
        }

        #endregion

        #region ShowClinicAppointments
        public async Task<Result<List<AppointmentResponse>>> ShowClinicAppointmentsAsync(int clinicId)
        {
            var repo = _unitOfWork.GetRepository<Appointment, int>();
            var spec = new ClinicAppointmentsSpec(clinicId);
            var appointments = await repo.GetAllWithSpecAsync(spec);
            if (appointments is null || !appointments.Any())
                return Result<List<AppointmentResponse>>.Fail(
                    Error.NotFound("Appointments.NotFound", "No appointments found for this clinic")
                );
            // ✅ Map to response DTOs
            var responses = appointments.Select(a => Map(a, a.TimeSlot)).ToList();
            return Result<List<AppointmentResponse>>.Ok(responses);
        }

        #endregion

        #region ShowPatintAppointments
        public async Task<Result<List<AppointmentResponse>>> ShowPatientAppointmentsAsync(string patientId)
        {
            var repo = _unitOfWork.GetRepository<Appointment, int>();
            var patientGuid = Guid.Parse(patientId);
            var spec = new PatientAppointmentsSpec(patientGuid);
            var appointments = await repo.GetAllWithSpecAsync(spec);
            if (appointments is null || !appointments.Any())
                return Result<List<AppointmentResponse>>.Fail(
                    Error.NotFound("Appointments.NotFound", "No appointments found for this patient")
                );
            // ✅ Map to response DTOs
            var responses = appointments.Select(a => Map(a, a.TimeSlot)).ToList();
            return Result<List<AppointmentResponse>>.Ok(responses);
        }

        #endregion

        #region Mapping

        private static AppointmentResponse Map(Appointment a, TimeSlot slot)
        {
            return new AppointmentResponse
            {
                Id = a.Id,
                ClinicId = a.ClinicId,
                TimeSlotId = a.TimeSlotId,
                PatientId = a.PatientId,
                Status = MapStatus(a.Status),

                Date = DateOnly.FromDateTime(slot.StartTime),
                StartTime = TimeOnly.FromDateTime(slot.StartTime),
                EndTime = TimeOnly.FromDateTime(slot.EndTime),
                PriceAtBooking = a.PriceAtBooking
            };
        }

        private static AppointmentStatusDto MapStatus(AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Pending => AppointmentStatusDto.Pending,
                AppointmentStatus.Confirmed => AppointmentStatusDto.Confirmed,
                AppointmentStatus.Cancelled => AppointmentStatusDto.Cancelled,
                _ => AppointmentStatusDto.Pending
            };
        }

        #endregion

    }
}
