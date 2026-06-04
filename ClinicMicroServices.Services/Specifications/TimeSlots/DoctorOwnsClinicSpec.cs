using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Specifications.TimeSlots
{
    public class DoctorOwnsClinicSpec : ISpecifications<DoctorClinic, int>
    {
        public ICollection<Expression<Func<DoctorClinic, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<DoctorClinic, object>>>();

        public Expression<Func<DoctorClinic, bool>> Critria { get; }

        public Expression<Func<DoctorClinic, object>> OrderBy { get; } = null;

        public Expression<Func<DoctorClinic, object>> OrderByDescending { get; } = null;

        public int Take { get; } = 0;

        public int Skip { get; } = 0;

        public bool IsPaginated { get; } = false;

        public DoctorOwnsClinicSpec(int clinicId, Guid doctorId)
        {
            Critria = c => c.Id == clinicId && c.DoctorId == doctorId;
        }
    }
}
