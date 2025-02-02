﻿namespace backend.Classes.Data
{
    using Npgsql;
    using System;

    /// <summary>
    /// Defines the <see cref="StatisticsRepository" />
    /// </summary>
    public class StatisticsRepository : IStatisticsRepository
    {
        /// <summary>
        /// Defines the configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsRepository"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/></param>
        public StatisticsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The GetStatistics
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <returns>The <see cref="List{Statistic}"/></returns>
        public List<Statistic> GetStatistics(int daysAgo, int userId)
        {
            var statistics = new List<Statistic>();

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var dateThreshold = DateTime.Now.AddDays(-daysAgo);

                var command = new NpgsqlCommand(
                    @"SELECT s.id, s.avgmovetime, s.numberofmoves, s.outcome, s.datetime,
                     g.playeroneid, g.playertwoid,
                     u1.username AS playerOneUsername,
                     u2.username AS playerTwoUsername
                    FROM tblstatistics s
                    INNER JOIN tblgames g ON g.id = s.gameid
                    INNER JOIN tblusers u1 ON u1.id = g.playeroneid
                    LEFT JOIN tblusers u2 ON u2.id = g.playertwoid
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

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
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
            var playerTwoId = reader.IsDBNull(reader.GetOrdinal("playertwoid"))
                              ? -1
                              : reader.GetInt32(reader.GetOrdinal("playertwoid"));

            return new Statistic
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                avgMoveTime = reader.GetDouble(reader.GetOrdinal("avgmovetime")),
                numberOfMoves = reader.GetInt32(reader.GetOrdinal("numberofmoves")),
                outcome = reader.GetString(reader.GetOrdinal("outcome")),
                datetime = reader.GetDateTime(reader.GetOrdinal("datetime")),
                playerOneUsername = reader.GetString(reader.GetOrdinal("playerOneUsername")),
                playerTwoUsername = playerTwoId == -1 ? null : reader.GetString(reader.GetOrdinal("playerTwoUsername"))
            };
        }
    }
}
