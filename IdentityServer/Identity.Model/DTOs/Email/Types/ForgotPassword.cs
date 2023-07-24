using Identity.Model.Constants.Email;
using Identity.Model.Interfaces;

namespace Identity.Model.DTOs.Email.Types
{
    public class ForgotPassword : IEmailClassifier
    {
        public bool Classified(Constants.Email.Type type) =>
            Constants.Email.Type.ForgotPassword == type;

        public ClassifiedEmail GetEmail(IDictionary<string, string> bodyParameters)
        {
            ClassifiedEmail classified = new()
            {
                Subject = Subject.PasswordReset,
                Body = Body.ForgotPassword(bodyParameters["url"])
            };

            return classified;
        }
    }
}
