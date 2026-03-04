using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.CommonResult
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Unauthorized = 3,
        Forbidden = 4,
        InvalidCredentials = 5,
        InternalServerError = 6,
        BadRequest = 7,
        Conflict = 8,
        ServiceUnavailable = 9,
        Timeout = 10,
        Unknown = 11
    }
}
