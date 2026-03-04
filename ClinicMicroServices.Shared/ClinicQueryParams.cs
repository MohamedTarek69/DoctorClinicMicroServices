using ClinicMicroServices.Shared.CommonResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared
{
    public class ClinicQueryParams
    {
        // Filters (اختياري)
        public Guid? DoctorId { get; set; }
        public int? ClinicId { get; set; }          // DoctorClinic.Id
        public Guid? PatientId { get; set; }        // للـ appointments
        public AppointmentStatusShared? Status { get; set; } // لو هتستخدمها مع appointments

        // Search (اختياري)
        public string? Search { get; set; }

        // Sorting
        public ClinicSortingOptions Sort { get; set; } = ClinicSortingOptions.Default;

        // Paging
        private int _pageIndex = 1;
        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value <= 0 ? 1 : value;
        }

        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 50; // غيرها زي ما تحب
        private int _pageSize = DefaultPageSize;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0) _pageSize = DefaultPageSize;
                else if (value > MaxPageSize) _pageSize = MaxPageSize;
                else _pageSize = value;
            }
        }
    }
}
