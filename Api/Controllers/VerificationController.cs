using Api.Helpers;
using Api.Models;
using Communication;
using Db;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class VerificationController : Controller
    {
        private readonly IVerificationCodeManager _verificationCodeManager;
        private readonly IVerificationManager _verificationReader;
        private readonly IEmailManager _emailManager;
        private readonly IConfiguration _configuration;

        public VerificationController(IVerificationCodeManager verificationCodeManager, IVerificationManager verificationReader, IEmailManager emailManager, IConfiguration configuration)
        {
            _verificationCodeManager = verificationCodeManager;
            _verificationReader = verificationReader;
            _emailManager = emailManager;
            _configuration = configuration;

        }

        [HttpPost("emailCode")]
        public void GenerateCode([FromBody] CodeForEmail email)
        {
            System.Diagnostics.Trace.WriteLine("Entering mail sending method");

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

            var response = sendGridClient.SendEmailAsync(msg);

            System.Diagnostics.Trace.WriteLine("-------------111------------------------ \n");
            System.Diagnostics.Debug.WriteLine("-------------222------------------------ \n");


            throw new System.Exception(response.Result.StatusCode.ToString());
            if (User.Identity.IsAuthenticated)
            {
                var code = _verificationCodeManager.AddNewCode(User.GetSocialEmail());
                _emailManager.SendVerificationCode(email.Email, code);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("verify/{code}")]
        public async Task<IActionResult> VerifyAsync(int code)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var (isCodeCorrect, ctsEmail) = _verificationCodeManager.CheckVerificationCode(User.GetSocialEmail(), code);
            if (isCodeCorrect)
            {
                await _verificationReader.AddNewVerificationAsync(User.GetSocialEmail(), ctsEmail);

                var user = User.Identity as ClaimsIdentity;
                user.AddClaim(new Claim("CtsEmail", ctsEmail));
                await HttpContext.SignInAsync(User);

                return Redirect(_configuration["LoginPageLink"]); 
            }
            return Unauthorized();
        }
    }
}