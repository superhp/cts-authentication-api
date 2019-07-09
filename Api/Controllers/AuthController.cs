using Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        [HttpGet("user")]
        public ActionResult<User> GetSignedInUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); 
            }
            return new User
            {
                Name = User.Identity.Name
            };            
        }
        
        [HttpGet("signin/{provider}")]
        public IActionResult SignIn(string provider, string returnUrl) =>
            Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, provider);

        [HttpGet("signout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(_configuration["LoginPageLink"]);
        }
    }
}