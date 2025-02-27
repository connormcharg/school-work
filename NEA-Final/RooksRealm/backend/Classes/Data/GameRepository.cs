namespace backend.Classes.Data
{
    using Npgsql;

    public class GameRepository : IGameRepository
    {
        private readonly IConfiguration configuration;

        public GameRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

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
    }
}