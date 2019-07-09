using System;
using System.Collections.Generic;
using System.Linq;

namespace Db
{
    public class VerificationCodeManager : IVerificationCodeManager
    {
        private readonly List<VerificationCode> _codes = new List<VerificationCode>();

        public int AddNewCode(string socialEmail)
        {
            var random = new Random();
            var randomCode = random.Next(100000, 999999);
            var verification = new VerificationCode()
            {
                SocialEmail = socialEmail,
                Code = randomCode
            };
            _codes.Add(verification);
            return randomCode;
        }

        public (bool, string) CheckVerificationCode(string socialEmail, int code)
        {
            var isCorrect = _codes.Any(x => x.SocialEmail == socialEmail && x.Code == code);
            return (isCorrect, _codes.Single(x => x.SocialEmail == socialEmail).CtsEmail);
        }
    }
}
