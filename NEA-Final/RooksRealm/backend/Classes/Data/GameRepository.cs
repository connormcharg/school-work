using Npgsql;

namespace backend.Classes.Data
{
    public class GameRepository : IGameRepository
    {
        public bool CreateGame(int playerOneId, int playerTwoId, string gameData)
        {
            if (gameData == null)
            {
                return false;
            }

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblGames (playerOneId, playerTwoId, gameData) VALUES (@playerOneId, @playerTwoId, @gameData);", 
                    connection);
                command.Parameters.AddWithValue("playerOneId", playerOneId);
                command.Parameters.AddWithValue("playerTwoId", playerTwoId);
                command.Parameters.AddWithValue("gameData", gameData);

                command.ExecuteNonQuery();
            }

            return true;
        }
    }
}
