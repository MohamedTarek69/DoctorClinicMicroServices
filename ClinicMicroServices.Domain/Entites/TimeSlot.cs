using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Domain.Entites
{
    public class TimeSlot : BaseEntity<int>
    {

        #region Properties

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        #endregion

        #region Navigation Properties

        [Required(ErrorMessage = "Clinic id is required.")]
        public int ClinicId { get; set; }

        public DoctorClinic Clinic { get; set; } = default!;

        // One-to-One (optional)
        public Appointment? Appointment { get; set; }

        #endregion

    }
}
