using CheckAndMate.Shared.Chess;

namespace CheckAndMate.Services
{
    public class ChessService
    {
        private Dictionary<string, Game> _games = new Dictionary<string, Game>();

        public Game? GetGame(string gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }

        public bool CreateGame(GameSettings gameSettings)
        {
            var game = new Game(gameSettings);
            return _games.TryAdd(game.id, game);
        }

        public bool RemoveGame(string gameId, bool archive)
        {
            _games.TryGetValue(gameId, out var game);
            if (game == null)
            {
                return false;
            }
            if (archive)
            {
                // send game to archive
            }
            return _games.Remove(gameId);
        } 
    }
}
