using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.Appointment
{
    public class CreateAppointmentRequest
    {
        public int TimeSlotId { get; set; }
    }
}
