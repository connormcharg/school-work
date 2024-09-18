using CheckAndMate.Hubs;
using CheckAndMate.Shared.Chess;
using Microsoft.AspNetCore.SignalR;

namespace CheckAndMate.Services
{
    public class ChessService
    {
        private Dictionary<string, Game> _games = new Dictionary<string, Game>();
        private readonly IHubContext<ChessHub> _hubContext;

        public ChessService(IHubContext<ChessHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public Game? GetGame(string gameId)
        {
            _games.TryGetValue(gameId, out var game);
            return game;
        }

        public List<Game> GetAllGames()
        {
            var allGames = _games.Values.ToList<Game>();
            return allGames;
        }

        public async void UpdateGame(string gameId, Game game)
        {
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            _games[gameId] = game;
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveGame", _games[gameId]);
        }

        public bool AddGame(Game game)
        {
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
