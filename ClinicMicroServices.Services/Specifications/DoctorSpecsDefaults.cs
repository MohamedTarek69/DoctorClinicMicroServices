using ClinicMicroServices.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Services.Specifications
{
    internal static class DoctorSpecsDefaults
    {
        public static Expression<Func<Doctor, bool>> TrueCriteria => d => true;
        public static Expression<Func<Doctor, object>> DefaultOrderBy => d => d.Id;
        public static Expression<Func<Doctor, object>> DefaultOrderByDesc => d => d.Id;
    }
}
