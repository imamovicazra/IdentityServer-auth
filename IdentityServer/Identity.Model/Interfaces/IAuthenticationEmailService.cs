using Identity.Model.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Interfaces
{
    public interface IAuthenticationEmailService
    {
        void SendAsync(Model.Constants.Email.Type type, string email, Dictionary<string, string> bodyParameters);
        ClassifiedEmail Classify(Model.Constants.Email.Type type, Dictionary<string, string> bodyParameters);
    }
}
