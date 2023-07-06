using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Responses
{
    public class ApiErrorException : Exception
    {
        public List<ApiError> Errors;

        public ApiErrorException(string code, string description)
        {
            Errors = new List<ApiError>
            {
                new ApiError(code, description)
            };
        }

        public ApiErrorException(ApiError error)
        {
            Errors = new List<ApiError>
            {
                error
            };
        }

        public ApiErrorException(List<ApiError> errors)
        {
            Errors = errors;
        }
    }
}
