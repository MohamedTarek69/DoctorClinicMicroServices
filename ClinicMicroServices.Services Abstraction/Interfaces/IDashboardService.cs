using ClinicMicroServices.Shared.CommonResult;
using ClinicMicroServices.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services_Abstraction.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<DashboardResponse>> GetClinicDashboardAsync(int clinicId);
        Task<Result<DashboardResponse>> GetDoctorDashboardAsync(string doctorId);
        Task<Result<DashboardResponse>> GetAdminDashboardAsync();
    }

}
