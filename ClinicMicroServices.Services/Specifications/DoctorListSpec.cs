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
    public class DoctorListSpec : ISpecifications<Doctor, Guid>
    {
        public ICollection<Expression<Func<Doctor, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<Doctor, object>>>();

        public Expression<Func<Doctor, bool>> Critria { get; private set; } = DoctorSpecsDefaults.TrueCriteria;

        public Expression<Func<Doctor, object>> OrderBy { get; private set; } = DoctorSpecsDefaults.DefaultOrderBy;
        public Expression<Func<Doctor, object>> OrderByDescending { get; private set; } = DoctorSpecsDefaults.DefaultOrderByDesc;

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaginated { get; private set; }

        public DoctorListSpec(ClinicQueryParams qp)
        {
            // Search
            if (!string.IsNullOrWhiteSpace(qp.Search))
            {
                var s = qp.Search.Trim().ToLower();
                Critria = d =>
                    d.DisplayName.ToLower().Contains(s) ||
                    d.Email.ToLower().Contains(s) ||
                    d.Specialty.ToLower().Contains(s);
            }

            // Sort (عدّل حسب الـ enum عندك)
            // لو عندك NameAsc/NameDesc في ClinicSortingOptions:
            switch (qp.Sort)
            {
                case ClinicSortingOptions.NameAsc:
                    OrderBy = d => d.DisplayName;
                    OrderByDescending = DoctorSpecsDefaults.DefaultOrderByDesc;
                    break;

                case ClinicSortingOptions.NameDesc:
                    OrderBy = DoctorSpecsDefaults.DefaultOrderBy;
                    OrderByDescending = d => d.DisplayName;
                    break;

                default:
                    // Default sort: name asc
                    OrderBy = d => d.DisplayName;
                    OrderByDescending = DoctorSpecsDefaults.DefaultOrderByDesc;
                    break;
            }

            // Paging
            IsPaginated = true;
            Skip = (qp.PageIndex - 1) * qp.PageSize;
            Take = qp.PageSize;
        }
    }
}
