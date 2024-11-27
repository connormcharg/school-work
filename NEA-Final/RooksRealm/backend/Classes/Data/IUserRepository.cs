namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="IUserRepository" />
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// The GetUserById
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="User?"/></returns>
        User? GetUserById(int id);

        /// <summary>
        /// The GetUserByUsername
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <returns>The <see cref="User?"/></returns>
        User? GetUserByUsername(string username);

        /// <summary>
        /// The GetUserByEmail
        /// </summary>
        /// <param name="email">The email<see cref="string"/></param>
        /// <returns>The <see cref="User?"/></returns>
        User? GetUserByEmail(string email);

        /// <summary>
        /// The CreateUser
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="email">The email<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <param name="role">The role<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool CreateUser(string username, string email, string password, string role = "user");

        /// <summary>
        /// The DeleteUser
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="email">The email<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool DeleteUser(string username, string email);

        /// <summary>
        /// The UpdateUserNickname
        /// </summary>
        /// <param name="email">The email<see cref="string"/></param>
        /// <param name="username">The username<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool UpdateUserNickname(string email, string username);

        /// <summary>
        /// The UpdateUserEmail
        /// </summary>
        /// <param name="email">The email<see cref="string"/></param>
        /// <param name="newEmail">The newEmail<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool UpdateUserEmail(string email, string newEmail);

        /// <summary>
        /// The UpdateUserPassword
        /// </summary>
        /// <param name="email">The email<see cref="string"/></param>
        /// <param name="oldStoredValue">The oldStoredValue<see cref="string"/></param>
        /// <param name="oldPassword">The oldPassword<see cref="string"/></param>
        /// <param name="newPassword">The newPassword<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool UpdateUserPassword(string email, string oldStoredValue, string oldPassword, string newPassword);

        /// <summary>
        /// The UpdateUserBoardTheme
        /// </summary>
        /// <param name="email">The email<see cref="string"/></param>
        /// <param name="newTheme">The newTheme<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool UpdateUserBoardTheme(string email, string newTheme);

        /// <summary>
        /// The UpdateUserRating
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="newRating">The newRating<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool UpdateUserRating(string username, int newRating);
    }
}
