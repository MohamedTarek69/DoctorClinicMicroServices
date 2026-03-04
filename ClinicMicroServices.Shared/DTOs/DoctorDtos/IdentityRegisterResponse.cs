using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.DoctorDtos
{
    public class IdentityRegisterResponse
    {
        public string Id { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
