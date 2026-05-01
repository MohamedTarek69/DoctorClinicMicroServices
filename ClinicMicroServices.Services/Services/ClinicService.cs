using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Services.Specifications.Clinics;
using ClinicMicroServices.Services.Specifications.Doctors;
using ClinicMicroServices.Services_Abstraction.Interfaces;
using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.ClinicDtos;

namespace ClinicMicroServices.Services.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ClinicResponse>> CreateClinicAsync(string identityUserId, CreateClinicRequest request)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();

            var doctor = await doctorRepo.GetByIdAsync(new DoctorByIdentityUserIdSpec(identityUserId)); // عدل الاسم هنا حسب الـ Doctor entity

            if (doctor is null)
                return Result<ClinicResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor profile not found for current user.")
                );
            if (!doctor.IsActive)
                return Result<ClinicResponse>.Fail(
                    Error.Validation("Doctor.Inactive", "Doctor is not active.")
                );
            var existingClinics = await clinicRepo.GetAllAsync(
                     new ClinicByNameAndDoctorSpec(request.ClinicName, doctor.Id)
                 );

            if (existingClinics.Any())
            {
                return Result<ClinicResponse>.Fail(
                    Error.Validation("Clinic.Duplicate", "Clinic already exists for this doctor.")
                );
            }

            var clinic = new DoctorClinic
            {
                DoctorId = doctor.Id,
                ClinicName = request.ClinicName,
                ClinicAddress = request.ClinicAddress,
                Description = request.Description
            };

            await clinicRepo.AddAsync(clinic);
            await _unitOfWork.SaveChangesAsync();

            return Result<ClinicResponse>.Ok(MapToResponse(clinic, doctor));
        }

        public async Task<Result<ClinicResponse>> GetClinicByIdAsync(int id)
        {
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();
            var clinic = await clinicRepo.GetByIdAsync(new ClinicByIdSpec(id));

            if (clinic is null)
                return Result<ClinicResponse>.Fail(
                    Error.NotFound("Clinic.NotFound", "Clinic not found.")
                );

            return Result<ClinicResponse>.Ok(MapToResponse(clinic));
        }

        public async Task<Result<PaginatedResult<ClinicResponse>>> GetClinicsAsync(ClinicQueryParams qp)
        {
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();

            var totalCount = await clinicRepo.CountAsync(new ClinicCountSpec(qp));
            var clinics = await clinicRepo.GetAllAsync(new ClinicListSpec(qp));

            var data = clinics.Select(MapToResponse);

            return Result<PaginatedResult<ClinicResponse>>.Ok(
                new PaginatedResult<ClinicResponse>(qp.PageIndex, qp.PageSize, totalCount, data)
            );
        }

        public async Task<Result<IEnumerable<ClinicResponse>>> GetClinicsByDoctorIdAsync(Guid doctorId)
        {
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();
            var clinics = await clinicRepo.GetAllAsync(new ClinicByDoctorIdSpec(doctorId));

            var result = clinics.Select(MapToResponse);
            return Result<IEnumerable<ClinicResponse>>.Ok(result);
        }

        public async Task<Result<IEnumerable<ClinicResponse>>> GetMyClinicsAsync(string identityUserId)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();

            var doctor = await doctorRepo.GetByIdAsync(new DoctorByIdentityUserIdSpec(identityUserId)); // عدل الاسم هنا

            if (doctor is null)
                return Result<IEnumerable<ClinicResponse>>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor profile not found for current user.")
                );

            var clinics = await clinicRepo.GetAllAsync(new ClinicByDoctorIdSpec(doctor.Id));
            var result = clinics.Select(MapToResponse);

            return Result<IEnumerable<ClinicResponse>>.Ok(result);
        }

        public async Task<Result<ClinicResponse>> UpdateClinicAsync(string identityUserId, int id, UpdateClinicRequest request)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();

            var doctor = await doctorRepo.GetByIdAsync(new DoctorByIdentityUserIdSpec(identityUserId)); // عدل الاسم هنا

            if (doctor is null)
                return Result<ClinicResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor profile not found for current user.")
                );

            var clinic = await clinicRepo.GetByIdAsync(id);

            if (clinic is null)
                return Result<ClinicResponse>.Fail(
                    Error.NotFound("Clinic.NotFound", "Clinic not found.")
                );

            if (clinic.DoctorId != doctor.Id)
                return Result<ClinicResponse>.Fail(
                    Error.Forbidden("Clinic.Forbidden", "You are not allowed to modify this clinic.")
                );

            if (!string.IsNullOrWhiteSpace(request.ClinicName))
                clinic.ClinicName = request.ClinicName;

            if (!string.IsNullOrWhiteSpace(request.ClinicAddress))
                clinic.ClinicAddress = request.ClinicAddress;

            if (request.Description is not null)
                clinic.Description = request.Description;

            clinicRepo.Update(clinic);
            await _unitOfWork.SaveChangesAsync();

            return Result<ClinicResponse>.Ok(MapToResponse(clinic));
        }

        public async Task<Result<ClinicResponse>> DeleteClinicAsync(string identityUserId, int id)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, Guid>();
            var clinicRepo = _unitOfWork.GetRepository<DoctorClinic, int>();

            var doctor = await doctorRepo.GetByIdAsync(new DoctorByIdentityUserIdSpec(identityUserId)); // عدل الاسم هنا

            if (doctor is null)
                return Result<ClinicResponse>.Fail(
                    Error.NotFound("Doctor.NotFound", "Doctor profile not found for current user.")
                );

            var clinic = await clinicRepo.GetByIdAsync(new ClinicByIdSpec(id));

            if (clinic is null)
                return Result<ClinicResponse>.Fail(
                    Error.NotFound("Clinic.NotFound", "Clinic not found.")
                );

            if (clinic.Appointments is not null && clinic.Appointments.Any())
                return Result<ClinicResponse>.Fail(
                    Error.Validation("Clinic.HasAppointments", "Cannot delete clinic with appointments.")
                );

            if (clinic.DoctorId != doctor.Id)
                return Result<ClinicResponse>.Fail(
                    Error.Forbidden("Clinic.Forbidden", "You are not allowed to delete this clinic.")
                );

            clinicRepo.Remove(clinic);
            await _unitOfWork.SaveChangesAsync();

            return Result<ClinicResponse>.Ok(MapToResponse(clinic));
        }

        private static ClinicResponse MapToResponse(DoctorClinic clinic) => new()
        {
            Id = clinic.Id,
            DoctorId = clinic.DoctorId,
            DoctorName = clinic.Doctor?.DisplayName,
            ClinicName = clinic.ClinicName,
            ClinicAddress = clinic.ClinicAddress,
            Description = clinic.Description
        };

        private static ClinicResponse MapToResponse(DoctorClinic clinic, Doctor? doctor = null) => new()
        {
            Id = clinic.Id,
            DoctorId = clinic.DoctorId,
            DoctorName = doctor?.DisplayName ?? clinic.Doctor?.DisplayName,
            ClinicName = clinic.ClinicName,
            ClinicAddress = clinic.ClinicAddress,
            Description = clinic.Description
        };
    }
}