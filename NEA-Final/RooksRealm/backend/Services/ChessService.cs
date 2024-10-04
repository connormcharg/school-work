using backend.Classes.Handlers;
using backend.Classes.State;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace backend.Services
{
    public class ChessService
    {
        private Dictionary<string, Game> games = new Dictionary<string, Game>();
        private readonly IHubContext<ChessHub> hubContext;

        public ChessService(IHubContext<ChessHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public Game? GetGame(string id)
        {
            if (games.TryGetValue(id, out var game))
            {
                return new Game(game);
            }
            return null;
        }

        public List<Game> GetAllGames()
        {
            return games.Values.Select(g => new Game(g)).ToList();
        }

        public async Task UpdateGame(string id, Game game)
        {
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            games[id] = game;
            var json = JsonConvert.SerializeObject(games[id]);
            await hubContext.Clients.Group(id).SendAsync("ReceiveGame", json);
        }

        public bool AddGame(Game game)
        {
            return games.TryAdd(game.id, new Game(game));
        }

        public bool RemoveGame(string id, bool archive = false)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                return false;
            }
            if (archive)
            {
                // send game to archive
            }
            return games.Remove(id);
        }
    }
}
