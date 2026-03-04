using ClinicMicroServices.Shared.CommonResult;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Presentation.Controllers
{
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleResult(Result result)
        {
            if (result is null)
                return StatusCode(500, new
                {
                    title = "Null Result",
                    status = 500,
                    detail = "Result was null.",
                    traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            if (result.IsSuccess)
                return Ok();

            return HandleErrors(result.Errors);
        }

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result is null)
                return StatusCode(500, new
                {
                    title = "Null Result",
                    status = 500,
                    detail = "Result was null.",
                    traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            if (result.IsSuccess)
                return Ok(result.Value);

            return HandleErrors(result.Errors);
        }

        protected IActionResult HandleModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                title = "Validation Error",
                status = 400,
                detail = "One or more validation errors occurred",
                traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                errors
            });
        }

        private IActionResult HandleErrors(IReadOnlyList<Error> errors)
        {
            if (errors is null || errors.Count == 0)
                return StatusCode(500, new
                {
                    title = "Unknown Error",
                    status = 500,
                    detail = "An unknown error occurred.",
                    traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });

            // Validation -> شكل ModelState-like
            if (errors.Any(e => e.ErrorType == ErrorType.Validation))
                return BuildValidationProblem(errors);

            var first = errors[0];

            var status = first.ErrorType switch
            {
                ErrorType.NotFound => 404,
                ErrorType.Unauthorized => 401,
                ErrorType.Forbidden => 403,
                ErrorType.Conflict => 409,
                ErrorType.BadRequest => 400,
                ErrorType.Timeout => 408,
                ErrorType.ServiceUnavailable => 503,
                ErrorType.InternalServerError => 500,
                ErrorType.InvalidCredentials => 401,
                _ => 400
            };

            return StatusCode(status, new
            {
                type = first.ErrorType.ToString(),
                title = first.Code,
                status,
                detail = first.Description,
                traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                errors = errors.Select(e => new
                {
                    e.Code,
                    e.Description,
                    type = e.ErrorType.ToString()
                })
            });
        }

        private IActionResult BuildValidationProblem(IReadOnlyList<Error> errors)
        {
            var grouped = errors
                .Where(e => e.ErrorType == ErrorType.Validation)
                .GroupBy(e => ExtractFieldName(e.Code))
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Description).ToArray()
                );

            return BadRequest(new
            {
                title = "Validation Error",
                status = 400,
                detail = "One or more validation errors occurred",
                traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                errors = grouped
            });
        }

        private static string ExtractFieldName(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "General";

            var idx = code.IndexOf('.');
            return idx > 0 ? code.Substring(0, idx) : "General";
        }
    }
}
