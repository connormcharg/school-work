namespace backend.Classes.Data
{
    public interface IUserRepository
    {
        User? GetUserById(int id);
        User? GetUserByUsername(string username);
        User? GetUserByEmail(string email);
        bool CreateUser(string username, string email, string password);
    }
}
