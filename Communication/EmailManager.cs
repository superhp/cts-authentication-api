﻿using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;

namespace Communication
{
    public class EmailManager : IEmailManager
    {
        private readonly IConfiguration _configuration;

        public EmailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendVerificationCode(string emailAddress, int code)
        {
            var msg = new SendGridMessage();
            msg.From = new EmailAddress(_configuration["SendGrid:From"], _configuration["SendGrid:FromName"]);
            var recipients = new List<EmailAddress>
            {
                new EmailAddress(emailAddress)
            };
            msg.AddTos(recipients);
            msg.Subject = "Verification";
            msg.AddContent(MimeType.Text, $"Your verification code is: {code}. You can activate your account by going to: {_configuration["CodeVerificationLink"]}{code}.");

            var sendGridClient = new SendGridClient(_configuration["SendGrid:APIKey"]);

            sendGridClient.SendEmailAsync(msg);

        }
    }
}
