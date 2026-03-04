using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Domain.Entites
{
    public class Doctor : BaseEntity<Guid>
    {
        #region Properties

        [Required(ErrorMessage = "Identity user id is required.")]
        public string IdentityUserId { get; set; } = default!;

        [Required(ErrorMessage = "Display name is required.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Display name must be between 3 and 100 characters.")]
        public string DisplayName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150, ErrorMessage = "Email must not exceed 150 characters.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(
            @"^(010|011|012|015)\d{8}$",
            ErrorMessage = "Phone number must be a valid Egyptian mobile number (010, 011, 012, 015) and 11 digits long."
        )]
        public string PhoneNumber { get; set; } = default!;

        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Specialty must be between 2 and 100 characters.")]
        public string Specialty { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        #endregion

        #region Navigation Properties

        public ICollection<DoctorClinic> DoctorClinics { get; set; } = [];

        #endregion


    }
}
