using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Specifications.Clinics
{
    public class ClinicCountSpec : ISpecifications<DoctorClinic, int>
    {
        public ICollection<Expression<Func<DoctorClinic, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<DoctorClinic, object>>>();

        public Expression<Func<DoctorClinic, bool>> Critria { get; private set; } = c => true;
        public Expression<Func<DoctorClinic, object>> OrderBy { get; } = c => c.Id;
        public Expression<Func<DoctorClinic, object>> OrderByDescending { get; } = c => c.Id;

        public int Take { get; } = 0;
        public int Skip { get; } = 0;
        public bool IsPaginated { get; } = false;

        public ClinicCountSpec(ClinicQueryParams qp)
        {
            if (qp.DoctorId.HasValue)
            {
                var doctorId = qp.DoctorId.Value;
                Critria = c => c.DoctorId == doctorId;
            }

            if (!string.IsNullOrWhiteSpace(qp.Search))
            {
                var s = qp.Search.Trim().ToLower();
                Critria = c =>
                    (!qp.DoctorId.HasValue || c.DoctorId == qp.DoctorId.Value) &&
                    (
                        c.ClinicName.ToLower().Contains(s) ||
                        c.ClinicAddress.ToLower().Contains(s) ||
                        (c.Description != null && c.Description.ToLower().Contains(s))
                    );
            }
        }
    }
}
