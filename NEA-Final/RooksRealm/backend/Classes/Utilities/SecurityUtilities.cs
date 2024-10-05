using backend.Classes.Security;
using System.Security.Cryptography;

namespace backend.Classes.Utilities
{
    public static class SecurityUtilities
    {       
        public static string GetHash(string text)
        {
            Bcrypt bcrypt = new Bcrypt();
            return bcrypt.BcryptHash(text);
        }

        public static bool VerifyPassword(string password, string storedValue)
        {
            Bcrypt bcrypt = new Bcrypt();
            return bcrypt.VerifyPassword(password, storedValue);
        }
    }
}
