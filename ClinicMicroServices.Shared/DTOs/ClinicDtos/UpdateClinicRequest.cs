using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.ClinicDtos
{
    public class UpdateClinicRequest
    {
        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string ClinicName { get; set; } = default!;

        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string ClinicAddress { get; set; } = default!;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
