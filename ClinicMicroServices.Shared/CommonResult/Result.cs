using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicMicroServices.Shared.CommonResult
{
    public class Result
    {
        private readonly List<Error> _errors = [];
        public bool IsSuccess => _errors.Count == 0;
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

        protected Result() { }

        protected Result(Error error)
        {
            _errors.Add(error);
        }

        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }

        public static Result Ok(bool v) => new Result();
        public static Result Fail(Error error) => new Result(error);
        public static Result Fail(List<Error> errors) => new Result(errors);
    }
}
