using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Communication
{
    public class EmailManager : IEmailManager
    {
        private readonly IConfiguration _configuration;

        public EmailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationCodeAsync(string emailAddress, int code)
        {
            var msg = new SendGridMessage();
            msg.From = new EmailAddress(_configuration["Email:From"], _configuration["Email:FromName"]);
            var recipients = new List<EmailAddress>
            {
                new EmailAddress(emailAddress)
            };
            msg.AddTos(recipients);
            msg.Subject = "Verification";
            msg.AddContent(MimeType.Text, $"Your verification code is: {code}. You can activate your account by going to: {_configuration["CodeVerificationLink"]}{code}.");

            var sendGridClient = new SendGridClient(_configuration["Email:ApiKey"]);
            var response = await sendGridClient.SendEmailAsync(msg);
        }
    }
}
