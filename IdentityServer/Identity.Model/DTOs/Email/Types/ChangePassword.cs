using Identity.Model.Constants.Email;
using Identity.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.DTOs.Email.Types
{
    public class ChangePassword : IEmailClassifier
    {
        public bool Classified(Constants.Email.Type type) =>
            Constants.Email.Type.ChangePassword == type;

        public ClassifiedEmail GetEmail(IDictionary<string, string> bodyParameters)
        {
            ClassifiedEmail classified = new()
            {
                Subject = Subject.PasswordReset,
                Body = Body.ChangePassword(bodyParameters["url"])
            };

            return classified;
        }
    }
}
