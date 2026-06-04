using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.TimeSlotDtos
{
    public class CreateTimeSlotRequest
    {
        //[Required]
        public int ClinicId { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        public int Capacity { get; set; } = 10;

        public decimal Price { get; set; }
    }
}
