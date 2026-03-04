using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Specifications
{
    public class DoctorCountSpec : ISpecifications<Doctor, Guid>
    {
        // ✅ لازم ICollection
        public ICollection<Expression<Func<Doctor, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<Doctor, object>>>();

        // ✅ interface says NOT nullable => لازم قيمة
        public Expression<Func<Doctor, bool>> Critria { get; private set; } = d => true;

        public Expression<Func<Doctor, object>> OrderBy { get; private set; } = d => d.Id;
        public Expression<Func<Doctor, object>> OrderByDescending { get; private set; } = d => d.Id;

        public int Take { get; private set; } = 0;
        public int Skip { get; private set; } = 0;
        public bool IsPaginated { get; private set; } = false;

        public DoctorCountSpec(ClinicQueryParams qp)
        {
            if (!string.IsNullOrWhiteSpace(qp.Search))
            {
                var s = qp.Search.Trim().ToLower();

                Critria = d =>
                    d.DisplayName.ToLower().Contains(s) ||
                    d.Email.ToLower().Contains(s) ||
                    d.Specialty.ToLower().Contains(s);
            }

            // CountSpec مش محتاج OrderBy/Paging/Includes، فسابينهم defaults
        }
    }
}
