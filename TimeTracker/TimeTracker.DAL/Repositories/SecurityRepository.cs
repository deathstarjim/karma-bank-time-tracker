using System;
using System.Text;
using TimeTracker.Core.Contracts;

namespace TimeTracker.DAL.Repositories
{
    public class SecurityRepository : ISecurity
    {
        public string EncryptPassword(string password)
        {
            byte[] encryptedPassword = Encoding.UTF8.GetBytes(password);

            return Convert.ToBase64String(encryptedPassword);
        }

        public string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        public string HashPassword(string encryptedPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(encryptedPassword);
        }

        public string HashPassword(string userPassword, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(userPassword, salt);
        }

        public bool PasswordIsValid(string submittedPassword, string retrievedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(submittedPassword, retrievedPassword);
        }
    }
}
