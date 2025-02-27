namespace backend.Classes.Data
{
    public interface IUserRepository
    {
        User? GetUserById(int id);

        User? GetUserByUsername(string username);

        User? GetUserByEmail(string email);

        bool CreateUser(string username, string email, string password, string role = "user");

        bool DeleteUser(string username, string email);

        bool UpdateUserNickname(string email, string username);

        bool UpdateUserEmail(string email, string newEmail);

        bool UpdateUserPassword(string email, string oldStoredValue, string oldPassword, string newPassword);

        bool UpdateUserBoardTheme(string email, string newTheme);

        bool UpdateUserRating(string username, int newRating);
    }
}