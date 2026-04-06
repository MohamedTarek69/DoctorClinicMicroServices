using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<DashboardResponse>> GetClinicDashboardAsync(int clinicId)
        {
            var appointmentRepo = _unitOfWork.GetRepository<Appointment, int>();
            var slotRepo = _unitOfWork.GetRepository<TimeSlot, int>();

            // Get appointments
            var appointments = await appointmentRepo.GetAllAsync();

            var clinicAppointments = appointments.Where(a => a.ClinicId == clinicId);

            var total = clinicAppointments.Count();
            var confirmed = clinicAppointments.Count(a => a.Status == AppointmentStatus.Confirmed);
            var cancelled = clinicAppointments.Count(a => a.Status == AppointmentStatus.Cancelled);

            // Get slots
            var slots = await slotRepo.GetAllAsync();
            var clinicSlots = slots.Where(s => s.ClinicId == clinicId);

            var availableSlots = clinicSlots.Count(s => s.Appointments.Count < s.Capacity);

            var result = new DashboardResponse
            {
                TotalAppointments = total,
                ConfirmedAppointments = confirmed,
                CancelledAppointments = cancelled,
                AvailableTimeSlots = availableSlots
            };

            return Result<DashboardResponse>.Ok(result);
        }

        public async Task<Result<DashboardResponse>> GetDoctorDashboardAsync(string doctorId)
        {

            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();
            var appointmentRepo = _unitOfWork.GetRepository<Appointment, int>();
            var slotRepo = _unitOfWork.GetRepository<TimeSlot, int>();

            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctor = await doctorRepo.GetAllAsync();

            var doctorEntity = doctor.FirstOrDefault(d => d.IdentityUserId == doctorId);

            // Get clinics of doctor
            var doctorClinics = await clinicRepo.GetAllAsync();

            Console.WriteLine($"DoctorId: {doctorId}");
            Console.WriteLine($"Clinics count: {doctorClinics.Count()}");

            var clinicIds = doctorClinics
                .Where(c => c.DoctorId == doctorEntity.Id)
                .Select(c => c.Id)
                .ToList();


            Console.WriteLine($"Filtered Clinics: {clinicIds.Count}");

            if (!clinicIds.Any())
            {
                return Result<DashboardResponse>.Ok(new DashboardResponse
                {
                    TotalAppointments = 0,
                    ConfirmedAppointments = 0,
                    CancelledAppointments = 0,
                    AvailableTimeSlots = 0
                });
            }

            // Get appointments
            var appointments = await appointmentRepo.GetAllAsync();

            var doctorAppointments = appointments
                .Where(a => clinicIds.Contains(a.ClinicId))
                .ToList();

            var total = doctorAppointments.Count;
            var confirmed = doctorAppointments.Count(a => a.Status == AppointmentStatus.Confirmed);
            var cancelled = doctorAppointments.Count(a => a.Status == AppointmentStatus.Cancelled);

            // Get slots
            var slots = await slotRepo.GetAllAsync();

            var doctorSlots = slots
                .Where(s => clinicIds.Contains(s.ClinicId))
                .ToList();

            var availableSlots = doctorSlots.Count(s => s.Appointments.Count < s.Capacity);

            var response = new DashboardResponse
            {
                TotalAppointments = total,
                ConfirmedAppointments = confirmed,
                CancelledAppointments = cancelled,
                AvailableTimeSlots = availableSlots
            };

            return Result<DashboardResponse>.Ok(response);
        }

        public async Task<Result<DashboardResponse>> GetAdminDashboardAsync()
        {
            var appointmentRepo = _unitOfWork.GetRepository<Appointment, int>();
            var slotRepo = _unitOfWork.GetRepository<TimeSlot, int>();

            // ✅ Get all appointments
            var appointments = await appointmentRepo.GetAllAsync();

            var total = appointments.Count();
            var confirmed = appointments.Count(a => a.Status == AppointmentStatus.Confirmed);
            var cancelled = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);

            // ✅ Get all slots
            var slots = await slotRepo.GetAllAsync();

            var availableSlots = slots.Count(s => s.Appointments.Count < s.Capacity);

            var response = new DashboardResponse
            {
                TotalAppointments = total,
                ConfirmedAppointments = confirmed,
                CancelledAppointments = cancelled,
                AvailableTimeSlots = availableSlots
            };

            return Result<DashboardResponse>.Ok(response);
        }

        private Guid GetDoctorIdFromToken()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var doctorIdClaim = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return Guid.Parse(doctorIdClaim!);
        }
    }
}
