using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.PatientDtos
{
    public class ReturnedAllergyDto
    {
        public int Id { get; set; }

        public string AllergyName { get; set; } = default!;
    }
}
