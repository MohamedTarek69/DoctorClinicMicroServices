using ClinicMicroServices.Shared;
using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.ClinicDtos;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface IClinicService
    {
        Task<Result<ClinicResponse>> CreateClinicAsync(string identityUserId, CreateClinicRequest request);
        Task<Result<ClinicResponse>> GetClinicByIdAsync(int id);
        Task<Result<PaginatedResult<ClinicResponse>>> GetClinicsAsync(ClinicQueryParams qp);
        Task<Result<IEnumerable<ClinicResponse>>> GetClinicsByDoctorIdAsync(Guid doctorId);
        Task<Result<IEnumerable<ClinicResponse>>> GetMyClinicsAsync(string identityUserId);
        Task<Result<ClinicResponse>> UpdateClinicAsync(string identityUserId, int id, UpdateClinicRequest request);
        Task<Result<ClinicResponse>> DeleteClinicAsync(string identityUserId, int id);
    }
}