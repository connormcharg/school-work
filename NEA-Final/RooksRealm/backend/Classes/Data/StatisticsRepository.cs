using Npgsql;

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
                    "INSERT INTO tblStatistics (numberOfMoves, userId, gameId, outcome) VALUES (@numberOfMoves, @userId, @gameId, @outcome);",
                    connection);
                command.Parameters.AddWithValue("numberOfMoves", numberOfMoves);
                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("gameId", gameId);
                command.Parameters.AddWithValue("outcome", outcome);

                command.ExecuteNonQuery();
            }

            return true;
        }
    }
}
