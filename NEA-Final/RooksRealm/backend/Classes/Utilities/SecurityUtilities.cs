using backend.Classes.Security;
using System.Security.Cryptography;

namespace backend.Classes.Utilities
{
    public static class SecurityUtilities
    {       
        public static string GetHash(string text)
        {
            Bcrypt bcrypt = new Bcrypt();
            return BitConverter.ToString(bcrypt.BcryptHash(text)).Replace("-", "").ToLower();
        }
    }
}
