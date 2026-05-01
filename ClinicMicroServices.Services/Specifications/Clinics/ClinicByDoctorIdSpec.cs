using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Clinics
{
    public class ClinicByDoctorIdSpec : ISpecifications<DoctorClinic, int>
    {
        public ICollection<Expression<Func<DoctorClinic, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<DoctorClinic, object>>>();

        public Expression<Func<DoctorClinic, bool>> Critria { get; }
        public Expression<Func<DoctorClinic, object>> OrderBy { get; } = c => c.ClinicName;
        public Expression<Func<DoctorClinic, object>> OrderByDescending { get; } = c => c.Id;

        public int Take { get; } = 0;
        public int Skip { get; } = 0;
        public bool IsPaginated { get; } = false;

        public ClinicByDoctorIdSpec(Guid doctorId)
        {
            Critria = c => c.DoctorId == doctorId;
            IncludeExpressions.Add(c => c.Doctor);
        }
    }
}