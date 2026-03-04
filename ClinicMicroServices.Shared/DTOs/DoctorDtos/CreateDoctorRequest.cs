using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.DoctorDtos
{
    public class CreateDoctorRequest
    {
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Specialty { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
