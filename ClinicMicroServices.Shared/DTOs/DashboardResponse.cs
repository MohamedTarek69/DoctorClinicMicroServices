using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.DTOs
{
    public class DashboardResponse
    {
        public int TotalAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        public int AvailableTimeSlots { get; set; }
    }
}
