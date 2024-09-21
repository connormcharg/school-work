using CheckAndMate.Hubs;
using CheckAndMate.Shared.Chess;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

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

        public async Task UpdateGame(string gameId, Game game)
        {
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            _games[gameId] = game;
            var json = JsonConvert.SerializeObject(_games[gameId]);
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveGame", json);
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
