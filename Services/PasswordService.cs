

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using padelya_api.Models;

namespace padelya_api.Services
{
    public class PasswordService : IPasswordService
    {
        public string GenerateRandomPassword(int length = 12)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            StringBuilder res = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (res.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }
            return res.ToString();
        }

        public string HashPassword(User? user, string password)
        {
            return new PasswordHasher<User>().HashPassword(user, password);
        }


        public bool VerifyPassword(User user, string hashedPassword, string password)
        {
            return new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }

}
