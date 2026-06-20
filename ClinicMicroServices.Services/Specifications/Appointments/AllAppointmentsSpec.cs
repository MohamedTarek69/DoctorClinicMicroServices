using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Appointments
{
    public class AllAppointmentsSpec : ISpecifications<Appointment, int>
    {
        public AllAppointmentsSpec()
        {
            IncludeExpressions = new List<Expression<Func<Appointment, object>>>
            {
                a => a.TimeSlot,
                a => a.Clinic
            };
        }

        public ICollection<Expression<Func<Appointment, object>>> IncludeExpressions { get; }

        public Expression<Func<Appointment, bool>> Critria
            => a => true;

        public Expression<Func<Appointment, object>> OrderBy
            => null!;

        public Expression<Func<Appointment, object>> OrderByDescending
            => null!;

        public int Take => 0;

        public int Skip => 0;

        public bool IsPaginated => false;
    }
}