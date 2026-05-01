using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.TimeSlots
{
    public class AvailableTimeSlotsSpec : ISpecifications<TimeSlot, int>
    {
        public ICollection<Expression<Func<TimeSlot, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<TimeSlot, object>>>();

        public Expression<Func<TimeSlot, bool>> Critria { get; }

        public Expression<Func<TimeSlot, object>> OrderBy { get; } = t => t.StartTime;
        public Expression<Func<TimeSlot, object>> OrderByDescending { get; } = t => t.Id;

        public int Take => 0;
        public int Skip => 0;
        public bool IsPaginated => false;

        public AvailableTimeSlotsSpec(int clinicId)
        {
            Critria = t => t.ClinicId == clinicId;

            IncludeExpressions.Add(t => t.Clinic);
            IncludeExpressions.Add(t => t.Appointments);
        }
    }
}
