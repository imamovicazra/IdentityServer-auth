using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Constants
{
    public class ErrorDescriptions
    {
        public const string UserDoesNotExistWithId = "User with that ID does not exist";
        public const string UserDoesNotExistWithEmail = "User with that email does not exist";
        public const string EmailNotConfirmed = "Email is not confirmed";
        public const string UserAlreadyVerified = "User is already verified";
        public const string IncorrectCredentials = "Incorrect credentials.";
        public const string InvalidGrantType = "Grant type is not supported.";
    }
}
