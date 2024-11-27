using Npgsql;

namespace backend.Classes.Data
{
    public class MessageRepository : IMessageRepository
    {
        public List<Message> GetMessages(int daysAgo)
        {
            var messages = new List<Message>();

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var dateThreshold = DateTime.Now.AddDays(-daysAgo);  // Get the date N days ago

                var command = new NpgsqlCommand(
                    @"SELECT m.id, m.title, m.content, u.username, m.datetime 
              FROM tblmessages m
              INNER JOIN tblusers u ON m.userid = u.id
              WHERE m.datetime >= @dateThreshold;",
                    connection);

                command.Parameters.AddWithValue("dateThreshold", dateThreshold);

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

        public bool CreateMessage(string title, string content, int userId, DateTime? dateTime = null)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) || userId == -1)
            {
                return false;
            }

            var timestamp = dateTime ?? DateTime.Now;

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblmessages (title, content, userid, datetime) VALUES (@title, @content, @userid, @datetime);",
                    connection);

                command.Parameters.AddWithValue("title", title);
                command.Parameters.AddWithValue("content", content);
                command.Parameters.AddWithValue("userid", userId);
                command.Parameters.AddWithValue("datetime", timestamp);

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
                username = reader.GetString(reader.GetOrdinal("username")),
                datetime = reader.GetDateTime(reader.GetOrdinal("datetime"))
            };
        }
    }
}
