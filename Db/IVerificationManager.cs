using System.Threading.Tasks;

namespace Db
{
    public interface IVerificationManager
    {
        Task AddNewVerificationAsync(string socialEmail, string ctsEmail);
        Task<string> GetCtsEmailAsync(string socialEmail);
    }
}