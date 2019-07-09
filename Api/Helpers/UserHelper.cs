using System.Linq;

namespace Api.Helpers
{
    public static class UserHelper
    {
        public static string GetSocialEmail(this System.Security.Claims.ClaimsPrincipal user) =>
            user.Claims.SingleOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
    }
}
