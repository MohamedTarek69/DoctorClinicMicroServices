using ClinicMicroServices.Domain.Contracts;
using ClinicMicroServices.Domain.Entites;
using ClinicMicroServices.Shared;
using System.Linq.Expressions;

namespace ClinicMicroServices.Services.Specifications.Clinics
{
    public class ClinicListSpec : ISpecifications<DoctorClinic, int>
    {
        public ICollection<Expression<Func<DoctorClinic, object>>> IncludeExpressions { get; }
            = new List<Expression<Func<DoctorClinic, object>>>();

        public Expression<Func<DoctorClinic, bool>> Critria { get; private set; } = c => true;
        public Expression<Func<DoctorClinic, object>> OrderBy { get; private set; } = c => c.ClinicName;
        public Expression<Func<DoctorClinic, object>> OrderByDescending { get; private set; } = c => c.Id;

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaginated { get; private set; }

        public ClinicListSpec(ClinicQueryParams qp)
        {
            IncludeExpressions.Add(c => c.Doctor);

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

            switch (qp.Sort)
            {
                case ClinicSortingOptions.NameAsc:
                    OrderBy = c => c.ClinicName;
                    break;

                case ClinicSortingOptions.NameDesc:
                    OrderByDescending = c => c.ClinicName;
                    break;

                default:
                    OrderBy = c => c.ClinicName;
                    break;
            }

            IsPaginated = true;
            Skip = (qp.PageIndex - 1) * qp.PageSize;
            Take = qp.PageSize;
        }
    }
}