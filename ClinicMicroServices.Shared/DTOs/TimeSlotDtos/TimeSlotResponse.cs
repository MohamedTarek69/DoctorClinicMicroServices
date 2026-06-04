using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.TimeSlotDtos
{
    public class TimeSlotResponse
    {
        public int Id { get; set; }
        public int ClinicId { get; set; }
        public string? ClinicName { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public int AvailableCount { get; set; }

        public decimal Price { get; set; }

    }
}
