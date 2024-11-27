namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="IGameRepository" />
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// The CreateGame
        /// </summary>
        /// <param name="playerOneId">The playerOneId<see cref="int"/></param>
        /// <param name="playerTwoId">The playerTwoId<see cref="int"/></param>
        /// <param name="gameData">The gameData<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
        int CreateGame(int playerOneId, int playerTwoId, string gameData);
    }
}
