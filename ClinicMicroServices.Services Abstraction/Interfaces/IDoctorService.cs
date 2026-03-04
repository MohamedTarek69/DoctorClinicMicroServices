using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.DoctorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface IDoctorService
    {
        Task<Result<DoctorResponse>> CreateDoctorAsync(CreateDoctorRequest request);
        Task<Result<DoctorResponse>> GetDoctorByIdAsync(Guid id, bool includeClinics = false);
        Task<Result<PaginatedResult<DoctorResponse>>> GetDoctorsAsync(ClinicQueryParams qp);
        Task<Result<DoctorResponse>> UpdateDoctorAsync(Guid id, UpdateDoctorRequest request);
        Task<Result<bool>> ActivateDoctorAsync(Guid id);
        Task<Result<bool>> DeactivateDoctorAsync(Guid id);
        Task<Result<bool>> UpdateDoctorPasswordAsync(Guid id, UpdateDoctorPasswordRequest newPassword);
        Task<bool> IsDoctorOwnerAsync(Guid doctorId, string identityUserId);
    }
}
