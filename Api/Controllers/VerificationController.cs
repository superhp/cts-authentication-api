﻿using Api.Helpers;
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

        [HttpGet("emailCode/{ctsEmail}")]
        public void GenerateCode(string ctsEmail)
        {
            if (User.Identity.IsAuthenticated)
            {
                var code = _verificationCodeManager.AddNewCode(User.GetSocialEmail());
                _emailManager.SendVerificationCode(ctsEmail, code);
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