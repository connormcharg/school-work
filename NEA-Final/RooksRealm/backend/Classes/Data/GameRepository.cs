using Npgsql;

namespace backend.Classes.Data
{
    public class GameRepository : IGameRepository
    {
        public int CreateGame(int playerOneId, int playerTwoId, string gameData)
        {
            if (gameData == null)
            {
                return -1;
            }

            using (var connection = new NpgsqlConnection(dbConstants.connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblgames (playeroneid, playertwoid, gamedata) VALUES (@playerOneId, @playerTwoId, @gameData) RETURNING id;", 
                    connection);
                command.Parameters.AddWithValue("playerOneId", playerOneId);
                command.Parameters.AddWithValue("playerTwoId", playerTwoId);
                command.Parameters.AddWithValue("gameData", gameData);

                var insertedId = (int?)command.ExecuteScalar();

                connection.Close();

                if (insertedId != null)
                {
                    return (int)insertedId;
                }
            }

            return -1;
        }

        private Game MapReaderToGame(NpgsqlDataReader reader)
        {
            return new Game
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                playerOneId = reader.GetInt32(reader.GetOrdinal("playeroneid")),
                playerTwoId = reader.GetInt32(reader.GetOrdinal("playertwoid")),
                gameData = reader.GetString(reader.GetOrdinal("gamedata"))
            };
        }
    }
}
