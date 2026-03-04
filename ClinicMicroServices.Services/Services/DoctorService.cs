using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Services.Specifications;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.DoctorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityClient _identityClient;

        public DoctorService(IUnitOfWork unitOfWork, IIdentityClient identityClient)
        {
            _unitOfWork = unitOfWork;
            _identityClient = identityClient;
        }

        public async Task<Result<DoctorResponse>> CreateDoctorAsync(CreateDoctorRequest request)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            // ✅ Duplicate check by Email (Clinic DB)
            var existing = await repo.GetByIdAsync(new DoctorByEmailSpec(request.Email));
            if (existing is not null)
                return Result<DoctorResponse>.Fail(
                    Error.Conflict("Doctor.EmailExists", "A doctor with this email already exists.")
                );

            // ✅ Call Identity to create user (Admin-Register)
            var identityResult = await _identityClient.RegisterDoctorAsync(request);
            if (identityResult.IsFailure)
                return Result<DoctorResponse>.Fail(identityResult.Errors.ToList());

            // ✅ Save in Clinic DB
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(), // أو سيبها لو EF بيولدها
                IdentityUserId = identityResult.Value,
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Specialty = request.Specialty,
                IsActive = true
            };

            await repo.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            return Result<DoctorResponse>.Ok(MapToResponse(doctor));
        }

        public async Task<Result<DoctorResponse>> GetDoctorByIdAsync(Guid id, bool includeClinics = false)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var spec = new DoctorByIdSpec(id, includeClinics);
            var doctor = await repo.GetByIdAsync(spec);

            if (doctor is null)
                return Result<DoctorResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found.")
                );

            return Result<DoctorResponse>.Ok(MapToResponse(doctor));
        }

        public async Task<Result<PaginatedResult<DoctorResponse>>> GetDoctorsAsync(ClinicQueryParams qp)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var totalCount = await repo.CountAsync(new DoctorCountSpec(qp));
            var doctors = await repo.GetAllAsync(new DoctorListSpec(qp));

            var data = doctors.Select(MapToResponse);

            return Result<PaginatedResult<DoctorResponse>>.Ok(
                new PaginatedResult<DoctorResponse>(qp.PageIndex, qp.PageSize, totalCount, data)
            );
        }

        public async Task<Result<DoctorResponse>> UpdateDoctorAsync(Guid id, UpdateDoctorRequest request)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctor = await repo.GetByIdAsync(id);
            if (doctor is null)
                return Result<DoctorResponse>.Fail(Error.NotFound("Doctor.NotFound", $"Doctor {id} not found"));

            // 1) Update Clinic DB fields
            doctor.DisplayName = request.DisplayName;
            doctor.Email = request.Email;
            doctor.PhoneNumber = request.PhoneNumber;
            doctor.Specialty = request.Specialty;
            doctor.IsActive = request.IsActive;

            repo.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            // 2) Update Identity user (same data)
            var identityUpdate = await _identityClient.UpdateDoctorAsync(doctor.IdentityUserId, new()
            {
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                //Password = request.Password // optional
            });

            if (identityUpdate.IsFailure)
                return Result<DoctorResponse>.Fail(identityUpdate.Errors.ToList());

            return Result<DoctorResponse>.Ok(MapToResponse(doctor));
        }

        public async Task<Result<bool>> ActivateDoctorAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctor = await repo.GetByIdAsync(id);
            if (doctor is null)
                return Result<bool>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found.")
                );

            if (doctor.IsActive)
                return Result<bool>.Fail(
                    Error.Validation("Doctor.AlreadyActive", "Doctor is already active.")
                );

            // Optional: Activate in Identity
            //var identityResult = await _identityClient.ActivateUserAsync(doctor.IdentityUserId);
            //if (identityResult.IsFailure)
            //    return Result<bool>.Fail(identityResult.Errors.ToList());

            doctor.IsActive = true;
            repo.Update(doctor);

            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> DeactivateDoctorAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctor = await repo.GetByIdAsync(id);
            if (doctor is null)
                return Result<bool>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found.")
                );
            if (!doctor.IsActive)
                return Result<bool>.Fail(
                    Error.Validation("Doctor.AlreadyInactive", "Doctor is already inactive.")
                );
            doctor.IsActive = false;
            repo.Update(doctor);
            await _unitOfWork.SaveChangesAsync();
            // Optional: Deactivate in Identity
            //var identityResult = await _identityClient.DeactivateUserAsync(doctor.IdentityUserId);
            //if (identityResult.IsFailure)
            //    return Result<bool>.Fail(identityResult.Errors.ToList());
            return Result<bool>.Ok(true);

        }

        private static DoctorResponse MapToResponse(Doctor d) => new DoctorResponse
        {
            Id = d.Id,
            IdentityUserId = d.IdentityUserId,
            DisplayName = d.DisplayName,
            Email = d.Email,
            PhoneNumber = d.PhoneNumber,
            Specialty = d.Specialty,
            IsActive = d.IsActive
        };

        public async Task<Result<bool>> UpdateDoctorPasswordAsync(Guid id, UpdateDoctorPasswordRequest newPassword)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();
            var doctor = await repo.GetByIdAsync(id);
            if (doctor is null)
                return Result<bool>.Fail(Error.NotFound("Doctor.NotFound", "Doctor not found."));

            var identityResult = await _identityClient.UpdatePasswordAsync(doctor.IdentityUserId, newPassword);
            if (identityResult.IsFailure)
                return Result<bool>.Fail(identityResult.Errors.ToList());

            return Result<bool>.Ok(true);
        }

        public async Task<bool> IsDoctorOwnerAsync(Guid doctorId, string identityUserId)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();
            var doctor = await repo.GetByIdAsync(doctorId);
            if (doctor is null) return false;
            return doctor.IdentityUserId == identityUserId;
        }
    }
}
