using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Clinics
{
    public class ClinicByIdSpec : ISpecifications<DoctorClinic, int>
    {
        public ICollection<Expression<Func<DoctorClinic, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<DoctorClinic, object>>>();

        public Expression<Func<DoctorClinic, bool>> Critria { get; }
        public Expression<Func<DoctorClinic, object>> OrderBy { get; } = c => c.Id;
        public Expression<Func<DoctorClinic, object>> OrderByDescending { get; } = c => c.Id;

        public int Take { get; } = 0;
        public int Skip { get; } = 0;
        public bool IsPaginated { get; } = false;

        public ClinicByIdSpec(int id)
        {
            Critria = c => c.Id == id;
            IncludeExpressions.Add(c => c.Doctor);
            IncludeExpressions.Add(c => c.Appointments);
        }
    }
}