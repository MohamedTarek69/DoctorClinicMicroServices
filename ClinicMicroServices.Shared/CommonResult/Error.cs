using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.CommonResult
{
    public class Error
    {
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ErrorType ErrorType { get; set; }

        private Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }

        #region Static Factory Methods
        public static Error Failure(string code = "General.Failure", string description = "Failure has occurred")
            => new(code, description, ErrorType.Failure);

        public static Error Validation(string code = "General.Validation", string description = "Validation error occurred")
            => new(code, description, ErrorType.Validation);

        public static Error NotFound(string code = "General.NotFound", string description = "Resource not found")
            => new(code, description, ErrorType.NotFound);

        public static Error Unauthorized(string code = "General.Unauthorized", string description = "Unauthorized access")
            => new(code, description, ErrorType.Unauthorized);

        public static Error Forbidden(string code = "General.Forbidden", string description = "Access forbidden")
            => new(code, description, ErrorType.Forbidden);

        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "Invalid credentials provided")
            => new(code, description, ErrorType.InvalidCredentials);

        public static Error InternalServerError(string code = "General.InternalServerError", string description = "An internal server error has occurred")
            => new(code, description, ErrorType.InternalServerError);

        public static Error BadRequest(string code = "General.BadRequest", string description = "Bad request")
            => new(code, description, ErrorType.BadRequest);

        public static Error Conflict(string code = "General.Conflict", string description = "Conflict occurred")
            => new(code, description, ErrorType.Conflict);

        public static Error ServiceUnavailable(string code = "General.ServiceUnavailable", string description = "Service unavailable")
            => new(code, description, ErrorType.ServiceUnavailable);

        public static Error Timeout(string code = "General.Timeout", string description = "Request timed out")
            => new(code, description, ErrorType.Timeout);

        public static Error Unknown(string code = "General.Unknown", string description = "An unknown error occurred")
            => new(code, description, ErrorType.Unknown);
        #endregion
    }
}
