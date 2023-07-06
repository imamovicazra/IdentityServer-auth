using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Responses
{
    public class ApplicationUserResponse
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public ApplicationUserResponse(string userId, string firstName, string lastName, string email, bool isEmailConfirmed)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            IsEmailConfirmed = isEmailConfirmed;
        }
        public ApplicationUserResponse() { }
    }
}
