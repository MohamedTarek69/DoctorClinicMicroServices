using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.PatientDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface IPatientClient
    {

        Task<Result<ReturnedPatientDetailsDto>>
         GetPatientDetailsByIdentityUserIdAsync(
             Guid identityUserId,
             string token);
    }
}
