using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Domain.Entites
{
    public class Appointment : BaseEntity<int>
    {
        #region Properties

        [Required(ErrorMessage = "Patient id is required.")]
        public Guid PatientId { get; set; } // reference to Patient microservice

        [Required(ErrorMessage = "Appointment status is required.")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        #endregion

        #region Navigation Properties

        [Required(ErrorMessage = "Clinic id is required.")]
        public int ClinicId { get; set; }

        public DoctorClinic Clinic { get; set; } = default!;

        [Required(ErrorMessage = "TimeSlot id is required.")]
        public int TimeSlotId { get; set; }

        public TimeSlot TimeSlot { get; set; } = default!;

        #endregion


    }
}
