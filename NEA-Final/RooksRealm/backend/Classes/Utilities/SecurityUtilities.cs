namespace backend.Classes.Utilities
{
    using backend.Classes.Security;

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