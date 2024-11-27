namespace backend.Classes.Utilities
{
    using backend.Classes.Security;

    /// <summary>
    /// Defines the <see cref="SecurityUtilities" />
    /// </summary>
    public static class SecurityUtilities
    {
        /// <summary>
        /// The GetHash
        /// </summary>
        /// <param name="text">The text<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetHash(string text)
        {
            Bcrypt bcrypt = new Bcrypt();
            return bcrypt.BcryptHash(text);
        }

        /// <summary>
        /// The VerifyPassword
        /// </summary>
        /// <param name="password">The password<see cref="string"/></param>
        /// <param name="storedValue">The storedValue<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool VerifyPassword(string password, string storedValue)
        {
            Bcrypt bcrypt = new Bcrypt();
            return bcrypt.VerifyPassword(password, storedValue);
        }
    }
}
