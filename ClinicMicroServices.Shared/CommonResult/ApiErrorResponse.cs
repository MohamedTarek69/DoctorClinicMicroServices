using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.CommonResult
{
    public class ApiErrorResponse
    {
        public string Title { get; set; } = "Error";
        public int Status { get; set; }
        public string Detail { get; set; } = "An error occurred";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
