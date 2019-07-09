using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;

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
            var mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(emailAddress));
            mailMsg.From = new MailAddress(_configuration["Smtp:From"], _configuration["Smtp:FromName"]);
            mailMsg.Subject = "Verification";
            var text = $"Your verification code is: {code}. You can activate your account by going to: {_configuration["CodeVerificationLink"]}{code}.";
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));

            var smtpClient = new SmtpClient(_configuration["Smtp:Server"], int.Parse(_configuration["Smtp:Port"]));
            var credentials = new System.Net.NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
            smtpClient.Credentials = credentials;

            smtpClient.Send(mailMsg);
        }
    }
}
