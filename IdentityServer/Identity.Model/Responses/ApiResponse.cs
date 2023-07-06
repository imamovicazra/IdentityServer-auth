using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Responses
{
    public class ApiResponse
    {
        public bool Succeeded { get; set; }

        public ApiResponse(bool isSuccesful)
        {
            Succeeded = isSuccesful;
        }
    }
}
