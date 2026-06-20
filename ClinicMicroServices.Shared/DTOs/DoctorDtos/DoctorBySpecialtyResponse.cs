using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.DoctorDtos
{
    public class DoctorBySpecialtyResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Specialty { get; set; } = null!;

        public List<string> Clinics { get; set; } = [];
    }
}
