using DentalClinic.Application.Interfaces;

namespace DentalClinic.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // BCrypt.Net-Next
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
