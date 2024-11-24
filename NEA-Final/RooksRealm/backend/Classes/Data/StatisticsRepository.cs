using Npgsql;
using System.Data;

namespace backend.Classes.Data
{
    public class StatisticsRepository : IStatisticsRepository
    {
        public bool CreateStatistic(int numberOfMoves, int userId, int gameId, string outcome)
        {
            if (outcome == null)
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblstatistics (numberofmoves, userid, gameid, outcome) VALUES (@numberOfMoves, @userId, @gameId, @outcome);",
                    connection);
                command.Parameters.AddWithValue("numberOfMoves", numberOfMoves);
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("gameId", gameId);
                command.Parameters.AddWithValue("outcome", outcome);

                command.ExecuteNonQuery();

                connection.Close();
            }

            return true;
        }

        private Statistic MapReaderToStatistic(NpgsqlDataReader reader)
        {
            return new Statistic
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                avgMoveTime = reader.GetDouble(reader.GetOrdinal("avgmovetime")),
                numberOfMoves = reader.GetInt32(reader.GetOrdinal("numberofmoves")),
                userId = reader.GetInt32(reader.GetOrdinal("userid")),
                gameId = reader.GetInt32(reader.GetOrdinal("gameid")),
                outcome = reader.GetString(reader.GetOrdinal("outcome"))
            };
        }
    }
}
