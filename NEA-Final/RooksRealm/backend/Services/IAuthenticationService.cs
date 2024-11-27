namespace backend.Services
{
    /// <summary>
    /// Defines the <see cref="IAuthenticationService" />
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// The Authenticate
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        string Authenticate(string username, string password);
    }
}
