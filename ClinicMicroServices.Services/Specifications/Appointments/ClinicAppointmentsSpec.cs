using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Appointments
{
    public class ClinicAppointmentsSpec : ISpecifications<Appointment, int>
    {
        private readonly int _clinicId;

        public ClinicAppointmentsSpec(int clinicId)
        {
            _clinicId = clinicId;
        }

        public ICollection<Expression<Func<Appointment, object>>> IncludeExpressions
            => new List<Expression<Func<Appointment, object>>>
            {
                a => a.TimeSlot
            };

        public Expression<Func<Appointment, bool>> Critria
            => a => a.ClinicId == _clinicId;

        public Expression<Func<Appointment, object>> OrderBy
            => a => a.Id;

        public Expression<Func<Appointment, object>> OrderByDescending => null;

        public int Take => 0;

        public int Skip => 0;

        public bool IsPaginated => false;
    }
}