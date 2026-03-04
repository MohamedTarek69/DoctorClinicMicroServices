using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.DoctorDtos
{
    public record UpdateDoctorPasswordRequest(string NewPassword = default!);
}
