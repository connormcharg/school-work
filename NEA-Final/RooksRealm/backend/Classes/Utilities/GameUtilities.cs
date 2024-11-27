namespace backend.Classes.Utilities
{
    using backend.Classes.State;

    /// <summary>
    /// Defines the <see cref="GameUtilities" />
    /// </summary>
    public static class GameUtilities
    {
        /// <summary>
        /// The IsPlayerWhite
        /// </summary>
        /// <param name="game">The game<see cref="Game"/></param>
        /// <param name="isHost">The isHost<see cref="bool"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsPlayerWhite(Game game, bool isHost)
        {
            if (game.players.Count == 1)
            {
                return !game.players[0].isWhite;
            }
            if (game.settings.isStartingWhite)
            {
                return isHost;
            }
            else
            {
                return !isHost;
            }
        }
    }
}
