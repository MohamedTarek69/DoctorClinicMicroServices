using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs.Appointment
{
    public class UpdateAppointmentStatusRequest
    {
        public int AppointmentId { get; set; }
        public AppointmentStatusDto Status { get; set; }
    }
}
