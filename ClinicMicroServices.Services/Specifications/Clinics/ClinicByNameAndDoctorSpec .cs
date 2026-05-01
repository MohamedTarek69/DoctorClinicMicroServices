using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Clinics
{
    public class ClinicByNameAndDoctorSpec : ISpecifications<DoctorClinic, int>
    {
        public ICollection<Expression<Func<DoctorClinic, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<DoctorClinic, object>>>();

        public Expression<Func<DoctorClinic, bool>> Critria { get; private set; }

        public Expression<Func<DoctorClinic, object>> OrderBy { get; private set; }
        public Expression<Func<DoctorClinic, object>> OrderByDescending { get; private set; }

        public int Take { get; private set; } = 0;
        public int Skip { get; private set; } = 0;
        public bool IsPaginated { get; private set; } = false;

        public ClinicByNameAndDoctorSpec(string clinicName, Guid doctorId)
        {
            var normalizedName = clinicName.Trim().ToLower();

            Critria = c =>
                c.DoctorId == doctorId &&
                c.ClinicName.ToLower() == normalizedName;
        }
    }
}