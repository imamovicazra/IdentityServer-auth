using Identity.Model.DTOs.Email;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Model.Interfaces
{
    public interface IEmailClassifier
    {
        bool Classified(Model.Constants.Email.Type type);
        ClassifiedEmail GetEmail(IDictionary<string, string> bodyParameters);
    }
}
