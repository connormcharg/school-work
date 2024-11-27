namespace backend.Classes.Data
{
    using Npgsql;
    using System;

    /// <summary>
    /// Defines the <see cref="StatisticsRepository" />
    /// </summary>
    public class StatisticsRepository : IStatisticsRepository
    {
        /// <summary>
        /// The GetStatistics
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <returns>The <see cref="List{Statistic}"/></returns>
        public List<Statistic> GetStatistics(int daysAgo, int userId)
        {
            var statistics = new List<Statistic>();

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var dateThreshold = DateTime.Now.AddDays(-daysAgo);

                var command = new NpgsqlCommand(
                    @"SELECT s.id, s.avgmovetime, s.numberofmoves, u.username, s.gameid, s.outcome, s.datetime 
                      FROM tblstatistics s
                      INNER JOIN tblusers u on u.id = s.userid
                      WHERE s.datetime >= @dateThreshold
                      AND s.userid = @userid;",
                    connection);

                command.Parameters.AddWithValue("dateThreshold", dateThreshold);
                command.Parameters.AddWithValue("userid", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        statistics.Add(MapReaderToStatistic(reader));
                    }
                }

                connection.Close();
            }

            return statistics;
        }

        /// <summary>
        /// The CreateStatistic
        /// </summary>
        /// <param name="numberOfMoves">The numberOfMoves<see cref="int"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <param name="gameId">The gameId<see cref="int"/></param>
        /// <param name="outcome">The outcome<see cref="string"/></param>
        /// <param name="dateTime">The dateTime<see cref="DateTime?"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool CreateStatistic(int numberOfMoves, int userId, int gameId, string outcome, DateTime? dateTime = null)
        {
            if (outcome == null)
            {
                return false;
            }

            var timestamp = dateTime ?? DateTime.Now;

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblstatistics (avgmovetime, numberofmoves, userid, gameid, outcome, datetime) VALUES (@avgmovetime, @numberOfMoves, @userId, @gameId, @outcome, @datetime);",
                    connection);
                command.Parameters.AddWithValue("avgmovetime", 0);
                command.Parameters.AddWithValue("numberOfMoves", numberOfMoves);
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("gameId", gameId);
                command.Parameters.AddWithValue("datetime", timestamp);
                command.Parameters.AddWithValue("outcome", outcome);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        /// <summary>
        /// The MapReaderToStatistic
        /// </summary>
        /// <param name="reader">The reader<see cref="NpgsqlDataReader"/></param>
        /// <returns>The <see cref="Statistic"/></returns>
        private Statistic MapReaderToStatistic(NpgsqlDataReader reader)
        {
            return new Statistic
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                avgMoveTime = reader.GetDouble(reader.GetOrdinal("avgmovetime")),
                numberOfMoves = reader.GetInt32(reader.GetOrdinal("numberofmoves")),
                username = reader.GetString(reader.GetOrdinal("username")),
                gameId = reader.GetInt32(reader.GetOrdinal("gameid")),
                outcome = reader.GetString(reader.GetOrdinal("outcome")),
                datetime = reader.GetDateTime(reader.GetOrdinal("datetime"))
            };
        }
    }
}
