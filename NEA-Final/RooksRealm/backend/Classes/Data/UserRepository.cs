﻿namespace backend.Classes.Data
{
    using backend.Classes.Utilities;
    using Npgsql;

    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration configuration;

        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public User? GetUserById(int id)
        {
            User? user = null;

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
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

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
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

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
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

        public bool CreateUser(string username, string email, string password, string role = "user")
        {
            if (username == null ||
                email == null ||
                password == null)
            {
                return false;
            }

            string storedHashValue = SecurityUtilities.GetHash(password);

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                // Check if the email already exists
                NpgsqlCommand checkEmailCommand = new NpgsqlCommand("SELECT COUNT(*) FROM tblusers WHERE email = @checkEmail", connection);
                checkEmailCommand.Parameters.AddWithValue("checkEmail", email);
                if (checkEmailCommand == null)
                {
                    return false;
                }
                int emailCount = Convert.ToInt32(checkEmailCommand.ExecuteScalar());
                if (emailCount > 0)
                {
                    return false;
                }

                var command = new NpgsqlCommand(
                    "INSERT INTO tblusers (username, email, emailconfirmed, storedhashvalue, boardtheme, rating, role) VALUES (@username, @email, @emailconfirmed, @storedhashvalue, @boardtheme, @rating, @role)",
                    connection);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("emailconfirmed", false);
                command.Parameters.AddWithValue("storedhashvalue", storedHashValue);
                command.Parameters.AddWithValue("boardtheme", "blue");
                command.Parameters.AddWithValue("rating", 400);
                command.Parameters.AddWithValue("role", role);

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

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "DELETE FROM tblusers WHERE username = @username AND email = @email;",
                    connection);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("email", email);

                command.ExecuteNonQuery();
            }

            return true;
        }

        public bool UpdateUserNickname(string email, string username)
        {
            if (email == null || username == null)
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "UPDATE tblusers SET username = @username WHERE email = @email;",
                    connection);
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("email", email);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        public bool UpdateUserEmail(string email, string newEmail)
        {
            if (email == null || newEmail == null)
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "UPDATE tblusers SET email = @newEmail WHERE email = @email;",
                    connection);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("newEmail", newEmail);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        public bool UpdateUserPassword(string email, string oldStoredValue, string oldPassword, string newPassword)
        {
            if (email == null || oldPassword == null || newPassword == null) { return false; }

            if (!SecurityUtilities.VerifyPassword(oldPassword, oldStoredValue))
            {
                return false;
            }

            string newStoredValue = SecurityUtilities.GetHash(newPassword);

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "UPDATE tblusers SET storedhashvalue = @storedhashvalue WHERE email = @email",
                    connection);
                command.Parameters.AddWithValue("storedhashvalue", newStoredValue);
                command.Parameters.AddWithValue("email", email);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        public bool UpdateUserBoardTheme(string email, string newTheme)
        {
            if (email == null || newTheme == null) { return false; }

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "UPDATE tblusers SET boardtheme = @boardtheme WHERE email = @email;",
                    connection);
                command.Parameters.AddWithValue("boardtheme", newTheme);
                command.Parameters.AddWithValue("email", email);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        public bool UpdateUserRating(string username, int newRating)
        {
            if (username == null) { return false; }

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "UPDATE tblusers SET rating = @rating WHERE username = @username;",
                    connection);
                command.Parameters.AddWithValue("rating", newRating);
                command.Parameters.AddWithValue("username", username);

                command.ExecuteNonQuery();

                connection.Close();
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
                storedHashValue = reader.GetString(reader.GetOrdinal("storedhashvalue")),
                boardTheme = reader.GetString(reader.GetOrdinal("boardtheme")),
                rating = reader.GetInt32(reader.GetOrdinal("rating")),
                role = reader.GetString(reader.GetOrdinal("role"))
            };
        }
    }
}