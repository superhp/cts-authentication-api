using Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Db;
using Api.Helpers;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVerificationManager _verificationManager;
        public AuthController(IConfiguration configuration, IVerificationManager verificationManager)
        {
            _configuration = configuration;
            _verificationManager = verificationManager;
        }

        [HttpGet("user")]
        public ActionResult<User> GetSignedInUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); 
            }
            var socialEmail = UserHelper.GetSocialEmail(User);
            var isVerified = _verificationManager.IsVerified(socialEmail);
            return new User
            {
                Name = User.Identity.Name,
                IsVerified = isVerified
            };            
        }
        
        [HttpGet("signin/{provider}")]
        public IActionResult SignIn(string provider, [FromQuery] string returnUrl) =>
            Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, provider);

        [HttpGet("signout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(_configuration["LoginPageLink"]);
        }
    }
}