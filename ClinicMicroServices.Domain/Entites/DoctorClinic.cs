using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Domain.Entites
{
    public class DoctorClinic : BaseEntity<int>
    {
        #region Properties

        [Required(ErrorMessage = "Clinic name is required.")]
        [StringLength(120, MinimumLength = 2,
            ErrorMessage = "Clinic name must be between 2 and 120 characters.")]
        public string ClinicName { get; set; } = default!;

        [Required(ErrorMessage = "Clinic address is required.")]
        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "Clinic address must be between 5 and 250 characters.")]
        public string ClinicAddress { get; set; } = default!;

        [StringLength(1000,
            ErrorMessage = "Clinic description must be a maximum of 1000 characters.")]
        public string? Description { get; set; }

        #endregion

        #region Navigation Properties

        [Required]
        public Guid DoctorId { get; set; }

        public Doctor Doctor { get; set; } = default!;

        public ICollection<TimeSlot> TimeSlots { get; set; } = [];
        public ICollection<Appointment> Appointments { get; set; } = [];

        #endregion

    }
}
