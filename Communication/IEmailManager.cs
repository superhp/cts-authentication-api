using System.Threading.Tasks;

namespace Communication
{
    public interface IEmailManager
    {
        Task SendVerificationCodeAsync(string emailAddress, int code);
    }
}