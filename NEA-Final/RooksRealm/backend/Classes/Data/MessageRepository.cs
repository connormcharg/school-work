using Npgsql;

namespace backend.Classes.Data
{
    public class MessageRepository : IMessageRepository
    {
        public List<Message> GetMessages()
        {
            var messages = new List<Message>();

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand("SELECT * FROM tblmessages;", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(MapReaderToMessage(reader));
                    }
                }

                connection.Close();
            }

            return messages;
        }

        public bool CreateMessage(string title, string content, int userId)
        {
            if (title == null || content == null || userId == -1)
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblmessages (title, content, userid) VALUES (@title, @content, @userid);", 
                    connection);

                command.Parameters.AddWithValue("title", title);
                command.Parameters.AddWithValue("content", content);
                command.Parameters.AddWithValue("userid", userId);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }


        private Message MapReaderToMessage(NpgsqlDataReader reader)
        {
            return new Message
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                title = reader.GetString(reader.GetOrdinal("title")),
                content = reader.GetString(reader.GetOrdinal("content")),
                userId = reader.GetInt32(reader.GetOrdinal("userid"))
            };
        }
    }
}
