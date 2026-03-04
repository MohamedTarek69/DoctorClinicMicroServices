using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.DoctorDtos
{
    public class DoctorResponse
    {
        public Guid Id { get; set; }
        public string IdentityUserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Specialty { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
