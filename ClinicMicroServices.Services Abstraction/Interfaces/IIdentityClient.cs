using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.DoctorDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface IIdentityClient
    {
        Task<Result<string>> RegisterDoctorAsync(CreateDoctorRequest request);
        Task<Result<UpdateIdentityUserResponse>> UpdateDoctorAsync(string userId, UpdateIdentityUserRequest request);
        Task<Result<bool>> UpdatePasswordAsync(string userId, UpdateDoctorPasswordRequest request);
        Task<bool> IsDoctorActiveAsync(string identityUserId);
    }
}

