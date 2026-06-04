using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.PatientDtos
{
    public class ReturnedPatientDetailsDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = default!;

        public string Address { get; set; } = default!;

        public DateOnly DateOfBirth { get; set; }

        public string Gender { get; set; } = default!;

        public string IdentityUserId { get; set; } = default!;

        public List<ReturnedMedicalRecordDto> MedicalRecords { get; set; }
            = [];

        public List<ReturnedAllergyDto> Allergies { get; set; }
            = [];
    }
}
