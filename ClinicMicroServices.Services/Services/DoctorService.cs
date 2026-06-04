using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Services.Specifications.Doctors;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.DoctorDtos;
using Microsoft.AspNetCore.Http;
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

        public async Task<Result<DoctorResponse>> UpdateDoctorAsync(Guid id, UpdateDoctorRequest request, string token)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctor = await repo.GetByIdAsync(id);
            if (doctor is null)
                return Result<DoctorResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", $"Doctor {id} not found"));

            // ✅ احتفظ بالقيم القديمة (علشان rollback)
            var oldIdentityData = new UpdateIdentityUserRequest
            {
                DisplayName = doctor.DisplayName,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber
            };

            // ✅ 1) Update Identity FIRST
            var identityUpdate = await _identityClient.UpdateDoctorAsync(
                doctor.IdentityUserId,
                new UpdateIdentityUserRequest
                {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                },token);

            if (identityUpdate.IsFailure)
                return Result<DoctorResponse>.Fail(identityUpdate.Errors.ToList());

            try
            {
                // ✅ 2) Update DB
                doctor.DisplayName = request.DisplayName;
                doctor.Email = request.Email;
                doctor.PhoneNumber = request.PhoneNumber;
                doctor.Specialty = request.Specialty;
                doctor.IsActive = request.IsActive;

                repo.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                // 🔥 3) Rollback Identity لو DB فشل
                await _identityClient.UpdateDoctorAsync(
                    doctor.IdentityUserId,
                    oldIdentityData,token);

                return Result<DoctorResponse>.Fail(
                    Error.Failure("Doctor.UpdateFailed", "Failed to update doctor. Changes rolled back."));
            }

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
                //Optional: Deactivate in Identity
               //var identityResult = await _identityClient.DeactivateUserAsync(d 
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

        public async Task<Result<bool>> UpdateDoctorPasswordAsync(Guid id, UpdateDoctorPasswordRequest newPassword, string token)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctor = await repo.GetByIdAsync(id);
            if (doctor is null)
                return Result<bool>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found."));

            // ❗ مفيش state قديم نرجعله في password
            // لكن نقدر نستخدم "best practice" rollback attempt (اختياري)

            // 1) Update Identity FIRST
            var identityResult = await _identityClient.UpdatePasswordAsync(
                doctor.IdentityUserId,
                newPassword,
                token);

            if (identityResult.IsFailure)
                return Result<bool>.Fail(identityResult.Errors.ToList());

            try
            {
                // 2) DB update (لو عندك audit أو flag مثلاً)
                // مفيش password في clinic DB غالبًا
                // بس ممكن نعمل logging / last updated


                repo.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                // ⚠️ "Compensation action"
                // مفيش rollback حقيقي للـ password إلا بإعادة تعيينه

                // (اختياري) إعادة تعيين password قديم لو عندك backup policy
                return Result<bool>.Fail(
                    Error.Failure("Doctor.PasswordUpdateFailed",
                    "Password updated in Identity but failed in DB sync."));
            }

            return Result<bool>.Ok(true);
        }

        public async Task<bool> IsDoctorOwnerAsync(Guid doctorId, string identityUserId)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();
            var doctor = await repo.GetByIdAsync(doctorId);
            if (doctor is null) return false;
            return doctor.IdentityUserId == identityUserId;
        }

        public async Task<Result<bool>> IsDoctorActiveByIdentityUserIdAsync(string identityUserId)
        {
            var repo = _unitOfWork.GetRepository<Doctor, Guid>();

            var doctors = await repo.GetAllAsync(new DoctorByIdentityUserIdSpec(identityUserId));
            var doctor = doctors.FirstOrDefault();

            if (doctor is null)
                return Result<bool>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor not found.")
                );

            return Result<bool>.Ok(doctor.IsActive);
        }
    }
}
