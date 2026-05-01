using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Appointments
{
    public class TimeSlotAvailableCheckSpec : ISpecifications<TimeSlot, int>
    {
        public ICollection<Expression<Func<TimeSlot, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<TimeSlot, object>>>();

        public Expression<Func<TimeSlot, bool>> Critria { get; }

        public Expression<Func<TimeSlot, object>> OrderBy { get; } = t => t.Id;
        public Expression<Func<TimeSlot, object>> OrderByDescending { get; } = t => t.Id;

        public int Take => 0;
        public int Skip => 0;
        public bool IsPaginated => false;

        public TimeSlotAvailableCheckSpec(int id)
        {
            Critria = t => t.Id == id;

            IncludeExpressions.Add(t => t.Appointments);
        }
    }
}