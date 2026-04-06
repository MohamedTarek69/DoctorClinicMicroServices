using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.ClinicDtos
{
    public class ClinicResponse
    {
        public int Id { get; set; }
        public string ClinicName { get; set; } = default!;
        public string ClinicAddress { get; set; } = default!;
        public string? Description { get; set; }
        public Guid DoctorId { get; set; }
        public string? DoctorName { get; set; }
    }
}
