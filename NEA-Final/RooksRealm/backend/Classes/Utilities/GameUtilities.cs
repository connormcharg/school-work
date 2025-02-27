﻿namespace backend.Classes.Utilities
{
    using backend.Classes.State;

    public static class GameUtilities
    {
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