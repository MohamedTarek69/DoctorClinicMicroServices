using ClinicMicroServices.Shared.DTOs.Appointment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.Appointment
{
    public class AppointmentResponse
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }
        public int TimeSlotId { get; set; }

        public Guid PatientId { get; set; }

        public AppointmentStatusDto Status { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public decimal PriceAtBooking { get; set; }
    }
}