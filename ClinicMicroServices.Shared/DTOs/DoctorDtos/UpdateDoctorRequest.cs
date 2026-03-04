using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.DoctorDtos
{
    public class UpdateDoctorRequest
    {
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Specialty { get; set; } = default!;
        public bool IsActive { get; set; }
        //public string? Password { get; set; } // optional
    }
}
