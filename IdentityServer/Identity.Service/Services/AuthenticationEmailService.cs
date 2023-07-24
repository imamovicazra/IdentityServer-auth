using Identity.Model.DTOs.Email;
using Identity.Model.Interfaces;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.Services
{
    public class AuthenticationEmailService : IAuthenticationEmailService
    {
        private readonly ILogger<AuthenticationEmailService> _logger;
        private readonly List<IEmailClassifier> _classifier;
        private readonly IEmailService _emailService;

        public AuthenticationEmailService(
            ILogger<AuthenticationEmailService> logger,
            List<IEmailClassifier> classifier,
            IEmailService emailService
            )
        {
            _logger = logger;
            _classifier = classifier;
            _emailService = emailService;
        }

        public ClassifiedEmail Classify(Model.Constants.Email.Type type, Dictionary<string, string> bodyParameters)
        {
            try
            {
                foreach (var classifier in _classifier)
                {
                    bool classified = classifier.Classified(type);
                    if (classified)
                        return classifier.GetEmail(bodyParameters);
                }

                throw new Exception("Email can not be classified"); // Move to error constants
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, nameof(Classify));
                throw ex;
            }
        }

        public async void SendAsync(Model.Constants.Email.Type type, string email, Dictionary<string, string> bodyParameters)
        {
            try
            {
                ClassifiedEmail classifiedEmail = Classify(type, bodyParameters);

                await _emailService.SendAsync(email, classifiedEmail.Subject, classifiedEmail.Body, classifiedEmail.IsHtml)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, nameof(SendAsync));
                throw ex;
            }
        }
    }
}
