using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.CommonResult
{
    public enum AppointmentStatusShared
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3,
        NoShow = 4
    }
}
