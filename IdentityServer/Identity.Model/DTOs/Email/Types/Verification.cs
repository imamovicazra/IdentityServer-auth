using Identity.Model.Constants.Email;
using Identity.Model.Interfaces;

namespace Identity.Model.DTOs.Email.Types
{
    public class Verification : IEmailClassifier
    {
        public bool Classified(Constants.Email.Type type) =>
            Constants.Email.Type.Verification == type;

        public ClassifiedEmail GetEmail(IDictionary<string, string> bodyParameters)
        {
            ClassifiedEmail classified = new()
            {
                Subject = Subject.Verification,
                Body = Body.Verification(bodyParameters["url"])
            };

            return classified;
        }
    }
}
