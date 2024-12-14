namespace backend.Classes.Data
{
    using Npgsql;

    /// <summary>
    /// Defines the <see cref="GameRepository" />
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private readonly IConfiguration configuration;
        
        public GameRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The CreateGame
        /// </summary>
        /// <param name="playerOneId">The playerOneId<see cref="int"/></param>
        /// <param name="playerTwoId">The playerTwoId<see cref="int"/></param>
        /// <param name="gameData">The gameData<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        public int CreateGame(int playerOneId, int playerTwoId, string gameData)
        {
            if (string.IsNullOrWhiteSpace(gameData))
            {
                return -1;
            }

            try
            {
                var parsedJson = Newtonsoft.Json.Linq.JToken.Parse(gameData);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return -1;
            }

            using (var connection = new NpgsqlConnection(dbConstants.GetConnectionString(configuration)))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO tblgames (playeroneid, playertwoid, gamedata) VALUES (@playerOneId, @playerTwoId, @gameData) RETURNING id;",
                    connection);
                command.Parameters.AddWithValue("playerOneId", playerOneId);
                command.Parameters.AddWithValue("playerTwoId", playerTwoId);
                command.Parameters.AddWithValue("gameData", NpgsqlTypes.NpgsqlDbType.Json, gameData);

                var insertedId = (int?)command.ExecuteScalar();

                connection.Close();

                if (insertedId != null)
                {
                    return (int)insertedId;
                }
            }

            return -1;
        }

        /// <summary>
        /// The MapReaderToGame
        /// </summary>
        /// <param name="reader">The reader<see cref="NpgsqlDataReader"/></param>
        /// <returns>The <see cref="Game"/></returns>
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
