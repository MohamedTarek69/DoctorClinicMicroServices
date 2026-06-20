using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Specifications.Doctors
{
    public class DoctorBySpecialtySpec : ISpecifications<Doctor, Guid>
    {
        private readonly string _specialty;

        public DoctorBySpecialtySpec(string specialty)
        {
            _specialty = specialty;
        }

        public Expression<Func<Doctor, bool>> Critria
            => d => d.Specialty == _specialty && d.IsActive;

        public ICollection<Expression<Func<Doctor, object>>> IncludeExpressions
            => new List<Expression<Func<Doctor, object>>>
            {
            d => d.DoctorClinics
            };

        public Expression<Func<Doctor, object>> OrderBy => null!;
        public Expression<Func<Doctor, object>> OrderByDescending => null!;
        public int Take => 0;
        public int Skip => 0;
        public bool IsPaginated => false;
    }
}
