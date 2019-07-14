using System.Threading.Tasks;

namespace Db
{
    public interface IVerificationManager
    {
        Task AddNewVerificationAsync(string socialEmail, string ctsEmail);
        string GetCtsEmail(string socialEmail);
        bool IsVerified(string socialEmail);
    }
}