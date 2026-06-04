using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.PatientDtos
{
    public class ReturnedMedicalRecordDto
    {
        public int Id { get; set; }

        public string DiseaseName { get; set; } = default!;

        public string? Notes { get; set; }
    }
}
