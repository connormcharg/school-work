﻿namespace backend.Classes.Data
{
    using Npgsql;

    /// <summary>
    /// Defines the <see cref="MessageRepository" />
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        /// <summary>
        /// Defines the configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        public MessageRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The GetMessages
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <returns>The <see cref="List{Message}"/></returns>
        public List<Message> GetMessages(int daysAgo)
        {
            var messages = new List<Message>();

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
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

        /// <summary>
        /// The CreateMessage
        /// </summary>
        /// <param name="title">The title<see cref="string"/></param>
        /// <param name="content">The content<see cref="string"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <param name="dateTime">The dateTime<see cref="DateTime?"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool CreateMessage(string title, string content, int userId, DateTime? dateTime = null)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) || userId == -1)
            {
                return false;
            }

            var timestamp = dateTime ?? DateTime.Now;

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
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

        /// <summary>
        /// The DeleteMessage
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool DeleteMessage(int id)
        {
            int rowsAffected;

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "DELETE FROM tblMessages WHERE id = @id;",
                    connection);

                command.Parameters.AddWithValue("id", id);

                rowsAffected = command.ExecuteNonQuery();

                connection.Close();
            }

            return rowsAffected >= 1;
        }

        /// <summary>
        /// The MapReaderToMessage
        /// </summary>
        /// <param name="reader">The reader<see cref="NpgsqlDataReader"/></param>
        /// <returns>The <see cref="Message"/></returns>
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
