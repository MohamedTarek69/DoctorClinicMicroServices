using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Specifications
{
    public class DoctorByIdentityUserIdSpec : ISpecifications<Doctor, Guid>
    {
        public ICollection<Expression<Func<Doctor, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<Doctor, object>>>();

        public Expression<Func<Doctor, bool>> Critria { get; private set; }

        public Expression<Func<Doctor, object>> OrderBy { get; private set; } = DoctorSpecsDefaults.DefaultOrderBy;
        public Expression<Func<Doctor, object>> OrderByDescending { get; private set; } = DoctorSpecsDefaults.DefaultOrderByDesc;

        public int Take { get; private set; } = 1;
        public int Skip { get; private set; } = 0;
        public bool IsPaginated { get; private set; } = true;

        public DoctorByIdentityUserIdSpec(string identityUserId)
        {
            var id = identityUserId.Trim();
            Critria = d => d.IdentityUserId == id;
        }
    }
}
