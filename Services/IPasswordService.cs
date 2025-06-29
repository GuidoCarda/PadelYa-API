using padelya_api.Models;

namespace padelya_api.Services
{
    public interface IPasswordService
    {
        string GenerateRandomPassword(int length = 12);
        string HashPassword(User? user, string password);
        bool VerifyPassword(User user, string hashedPassword, string password);
    }
}