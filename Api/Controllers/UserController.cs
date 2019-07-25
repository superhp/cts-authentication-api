using Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Db;
using Api.Helpers;
using System;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserManager _userManager;
        public UserController( IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("ctsEmail={ctsEmail}")]
        public ActionResult<Guid> GetUserGuid(string ctsEmail)
        {
            var userGuid = _userManager.GetUserGuid(ctsEmail);
            return userGuid;
        }
    }
}