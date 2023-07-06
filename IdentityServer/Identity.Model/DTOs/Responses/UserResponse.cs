using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Responses
{
    public class UserResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
