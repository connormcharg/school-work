using Npgsql;
using backend.Classes.Utilities;

namespace backend.Classes.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString =
            "Host=localhost;Database=rooksrealm;Username=root;Password=root";

        public User? GetUserById(int id)
        {
            User? user = null;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand("SELECT * FROM tblusers WHERE id = @id", connection);
                command.Parameters.AddWithValue("id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = MapReaderToUser(reader);
                    }
                }
            }

            return user;
        }

        public User? GetUserByUsername(string username)
        {
            User? user = null;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand("SELECT * FROM tblusers WHERE username = @username", connection);
                command.Parameters.AddWithValue("username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = MapReaderToUser(reader);
                    }
                }
            }

            return user;
        }

        public User? GetUserByEmail(string email)
        {
            User? user = null;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand("SELECT * FROM tblusers WHERE email = @email", connection);
                command.Parameters.AddWithValue("email", email);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = MapReaderToUser(reader);
                    }
                }
            }

            return user;
        }

        public bool CreateUser(string username, string email, string password)
        {            
            if (username == null ||
                email == null ||
                password == null)
            {
                return false;
            }

            string storedHashValue = SecurityUtilities.GetHash(password);

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Check if the email already exists
                NpgsqlCommand checkEmailCommand = new NpgsqlCommand("SELECT COUNT(*) FROM tblusers WHERE email = @checkEmail", connection);
                checkEmailCommand.Parameters.AddWithValue("checkEmail", email);
                if (checkEmailCommand == null)
                {
                    return false;
                }
                int emailCount = (int)checkEmailCommand.ExecuteScalar();
                if (emailCount > 0)
                {
                    return false;
                }

                var command = new NpgsqlCommand(
                    "INSERT INTO tblusers (username, email, emailconfirmed, storedhashvalue) VALUES (@username, @email, @emailconfirmed, @storedhashvalue)",
                    connection);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("emailconfirmed", false);
                command.Parameters.AddWithValue("storedhashvalue", storedHashValue);

                command.ExecuteNonQuery();
            }

            return true;
        }

        public bool DeleteUser(string username, string email)
        {
            if (username == null || email == null)
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "DELETE FROM tblUsers WHERE username = @username AND email = @email;",
                    connection);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("email", email);

                command.ExecuteNonQuery();
            }

            return true;
        } 

        private User MapReaderToUser(NpgsqlDataReader reader)
        {
            return new User
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                email = reader.GetString(reader.GetOrdinal("email")),
                emailConfirmed = reader.GetBoolean(reader.GetOrdinal("emailconfirmed")),
                username = reader.GetString(reader.GetOrdinal("username")),
                storedHashValue = reader.GetString(reader.GetOrdinal("storedhashvalue"))
            };
        }
    }
}
