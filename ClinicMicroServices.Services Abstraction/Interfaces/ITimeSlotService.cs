using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs.TimeSlotDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface ITimeSlotService
    {
        Task<Result<TimeSlotResponse>> CreateAsync(CreateTimeSlotRequest request, string doctorId);
        Task<Result<IEnumerable<TimeSlotResponse>>> GetByClinicAsync(int clinicId);
        Task<Result<IEnumerable<TimeSlotResponse>>> GetAvailableAsync(int clinicId);
        Task<Result<bool>> DeleteAsync(int id, string doctorId);
        Task<Result<TimeSlotResponse>> UpdateAsync(int id, UpdateTimeSlotRequest request, string doctorId);
    }
}
