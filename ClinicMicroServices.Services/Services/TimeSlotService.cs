using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Services.Specifications.Doctors;
using ClinicMicroServices.Services.Specifications.TimeSlots;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.TimeSlotDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TimeSlotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Create

        public async Task<Result<TimeSlotResponse>> CreateAsync(CreateTimeSlotRequest request, string doctorId)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("CREATE TIMESLOT REQUEST");
            Console.WriteLine("========================================");
            Console.WriteLine($"ClinicId = {request.ClinicId}");
            Console.WriteLine($"StartTime = {request.StartTime}");
            Console.WriteLine($"EndTime = {request.EndTime}");
            Console.WriteLine($"Capacity = {request.Capacity}");
            Console.WriteLine($"Price = {request.Price}");
            Console.WriteLine($"DoctorId From Token = {doctorId}");

            var repo = _unitOfWork.GetRepository<TimeSlot, int>();

            // 🔹 Get Doctor from IdentityUserId
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctorSpec = new DoctorByIdentityUserIdSpec(doctorId);
            var doctors = await doctorRepo.GetAllAsync(doctorSpec);
            var doctor = doctors.FirstOrDefault();

            Console.WriteLine($"Doctor Entity Id = {doctor?.Id}");

            if (doctor is null)
            {
                Console.WriteLine("Doctor NOT FOUND");
                return Result<TimeSlotResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found for this user.")
                );
            }

            // 🔹 Ownership validation
            var isOwner = await IsDoctorOwnerOfClinic(request.ClinicId, doctor.Id);

            Console.WriteLine($"IsOwner = {isOwner}");

            if (!isOwner)
            {
                Console.WriteLine("FORBIDDEN - Doctor is not owner of clinic");
                return Result<TimeSlotResponse>.Fail(
                    Error.Forbidden("TimeSlot.Forbidden", "You are not allowed to add slots to this clinic.")
                );
            }

            // 🔹 Validations
            if (request.EndTime <= request.StartTime)
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.InvalidRange", "End time must be greater than start time.")
                );

            if (request.StartTime < DateTime.UtcNow)
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.Past", "Cannot create timeslot in the past.")
                );

            if (request.Capacity <= 0)
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.InvalidCapacity", "Capacity must be greater than zero.")
                );

            if (request.Price < 0)
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.InvalidPrice", "Price cannot be negative.")
                );

            // 🔹 Overlap check
            var overlapSpec = new TimeSlotOverlapSpec(
                request.ClinicId,
                request.StartTime,
                request.EndTime
            );

            var exists = await repo.AnyAsync(overlapSpec);

            if (exists)
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.Overlap", "This slot overlaps with existing one.")
                );

            // 🔹 Create Slot
            var slot = new TimeSlot
            {
                ClinicId = request.ClinicId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Capacity = request.Capacity,
                Price = request.Price
            };

            await repo.AddAsync(slot);
            await _unitOfWork.SaveChangesAsync();

            Console.WriteLine($"SUCCESS - Slot Created. Id = {slot.Id}");

            return Result<TimeSlotResponse>.Ok(Map(slot, null));
        }

        #endregion

        #region Get By Clinic

        public async Task<Result<IEnumerable<TimeSlotResponse>>> GetByClinicAsync(int clinicId)
        {
            var repo = _unitOfWork.GetRepository<TimeSlot, int>();

            var spec = new TimeSlotByClinicSpec(clinicId);
            var slots = await repo.GetAllAsync(spec);

            var result = slots.Select(s => Map(s, s.Clinic));

            return Result<IEnumerable<TimeSlotResponse>>.Ok(result);
        }

        #endregion

        #region Get Available

        public async Task<Result<IEnumerable<TimeSlotResponse>>> GetAvailableAsync(int clinicId)
        {
            var repo = _unitOfWork.GetRepository<TimeSlot, int>();

            var spec = new AvailableTimeSlotsSpec(clinicId);
            var slots = await repo.GetAllAsync(spec);

            var result = slots
                .Where(s => s.Appointments.Count < s.Capacity)
                .Select(s => Map(s, s.Clinic));

            return Result<IEnumerable<TimeSlotResponse>>.Ok(result);
        }

        #endregion

        #region Delete

        public async Task<Result<bool>> DeleteAsync(int id, string doctorId)
        {
            var repo = _unitOfWork.GetRepository<TimeSlot, int>();

            var slot = await repo.GetByIdAsync(id);

            if (slot is null)
                return Result<bool>.Fail(
                    Error.NotFound("TimeSlot.NotFound", $"TimeSlot {id} not found")
                );

            // 🔹 1. Get Doctor from IdentityUserId
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctorSpec = new DoctorByIdentityUserIdSpec(doctorId);
            var doctors = await doctorRepo.GetAllAsync(doctorSpec);
            var doctor = doctors.FirstOrDefault();

            if (doctor is null)
                return Result<bool>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found for this user.")
                );

            // 🔹 2. Ownership validation
            var isOwner = await IsDoctorOwnerOfClinic(slot.ClinicId, doctor.Id);

            if (!isOwner)
                return Result<bool>.Fail(
                    Error.Forbidden("TimeSlot.Forbidden", "You are not allowed to delete this slot.")
                );

            // 🔹 3. Business rule
            if (slot.Appointments.Any())
                return Result<bool>.Fail(
                    Error.Validation("TimeSlot.Booked", "Cannot delete booked slot")
                );

            repo.Remove(slot);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }

        #endregion

        #region UpdateTimeSlot

        public async Task<Result<TimeSlotResponse>> UpdateAsync(int id, UpdateTimeSlotRequest request, string doctorId)
        {
            var repo = _unitOfWork.GetRepository<TimeSlot, int>();

            var slot = await repo.GetByIdAsync(id);

            if (slot is null)
                return Result<TimeSlotResponse>.Fail(
                    Error.NotFound("TimeSlot.NotFound", $"TimeSlot {id} not found")
                );

            // 🔹 1. Get Doctor from IdentityUserId
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctorSpec = new DoctorByIdentityUserIdSpec(doctorId);
            var doctors = await doctorRepo.GetAllAsync(doctorSpec);
            var doctor = doctors.FirstOrDefault();

            if (doctor is null)
                return Result<TimeSlotResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found for this user.")
                );

            // 🔹 2. Ownership validation
            var isOwner = await IsDoctorOwnerOfClinic(slot.ClinicId, doctor.Id);

            if (!isOwner)
                return Result<TimeSlotResponse>.Fail(
                    Error.Forbidden("TimeSlot.Forbidden", "You are not allowed to update this slot.")
                );

            // 🔹 3. Business rules
            if (slot.Appointments.Any())
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.Booked", "Cannot update a booked slot")
                );

            if (request.StartTime.HasValue)
                slot.StartTime = request.StartTime.Value;

            if (request.EndTime.HasValue)
                slot.EndTime = request.EndTime.Value;

            if (slot.EndTime <= slot.StartTime)
                return Result<TimeSlotResponse>.Fail(
                    Error.Validation("TimeSlot.InvalidRange", "End time must be greater than start time.")
                );

            if (request.Capacity.HasValue)
            {
                if (request.Capacity.Value <= 0)
                    return Result<TimeSlotResponse>.Fail(
                        Error.Validation("TimeSlot.InvalidCapacity", "Capacity must be greater than zero.")
                    );

                if (request.Capacity.Value < slot.Appointments.Count)
                    return Result<TimeSlotResponse>.Fail(
                        Error.Validation("TimeSlot.CapacityConflict", "Capacity cannot be less than booked appointments.")
                    );

                slot.Capacity = request.Capacity.Value;
            }

            if (request.Price.HasValue)
            {
                if (request.Price.Value < 0)
                    return Result<TimeSlotResponse>.Fail(
                        Error.Validation("TimeSlot.InvalidPrice", "Price cannot be negative.")
                    );

                slot.Price = request.Price.Value;
            }

            repo.Update(slot);
            await _unitOfWork.SaveChangesAsync();

            return Result<TimeSlotResponse>.Ok(Map(slot, null));
        }

        #endregion

        #region Mapping

        private static TimeSlotResponse Map(TimeSlot s, DoctorClinic? clinic)
        {
            var booked = s.Appointments?.Count ?? 0;

            return new TimeSlotResponse
            {
                Id = s.Id,
                ClinicId = s.ClinicId,
                ClinicName = clinic?.ClinicName,

                Date = DateOnly.FromDateTime(s.StartTime),
                StartTime = TimeOnly.FromDateTime(s.StartTime),
                EndTime = TimeOnly.FromDateTime(s.EndTime),

                Capacity = s.Capacity,
                BookedCount = booked,
                AvailableCount = s.Capacity - booked,

                Price = s.Price
            };
        }

        #endregion

        #region Helper
        private async Task<bool> IsDoctorOwnerOfClinic(int clinicId, Guid doctorId)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("OWNERSHIP CHECK");
            Console.WriteLine("========================================");
            Console.WriteLine($"Requested ClinicId = {clinicId}");
            Console.WriteLine($"Current DoctorId = {doctorId}");

            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();

            var clinic = await clinicRepo.GetByIdAsync(clinicId);

            if (clinic is null)
            {
                Console.WriteLine("Clinic NOT FOUND");
                return false;
            }

            Console.WriteLine($"Clinic Id = {clinic.Id}");
            Console.WriteLine($"Clinic DoctorId = {clinic.DoctorId}");

            var isOwner = clinic.DoctorId == doctorId;

            Console.WriteLine($"Is Owner = {isOwner}");

            return isOwner;
        }
        #endregion
    }
}
