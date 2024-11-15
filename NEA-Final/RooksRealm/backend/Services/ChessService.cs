using backend.Classes.Handlers;
using backend.Classes.State;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;

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

        // Better update functions

        private async Task UpdateClients(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var json = JsonConvert.SerializeObject(game);
            await hubContext.Clients.Group(id).SendAsync("ReceiveGame", json);
        }

        public async Task StartTimer(string id, bool isWhiteTimer)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (isWhiteTimer)
            {
                game.state.whiteTimeRunning = true;
            }
            else
            {
                game.state.blackTimeRunning = true;
            }
            await UpdateClients(id);
        }

        public async Task StopTimers(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.state.whiteTimeRunning = false;
            game.state.blackTimeRunning = false;
            await UpdateClients(id);
        }

        public async Task UpdateTimers(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.whiteTimeRunning)
            {
                game.state.whiteTime -= 1;
            }
            if (game.state.blackTimeRunning)
            {
                game.state.blackTime -= 1;
            }
            if (game.state.whiteTime <= 0 || game.state.blackTime <= 0)
            {
                game.state.gameOver = true;
            }
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        public async Task EngineUpdate(string id, int moveId, int suggestedMoveId)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            var move = game.currentValidMoves.FirstOrDefault(m => m.moveID == moveId);
            if (move != null)
            {
                GameHandler.MakeMove(game, move);
                game.suggestedMoveId = suggestedMoveId;
                game.currentValidMoves = GameHandler.FindValidMoves(game);
                GameHandler.CheckGameOver(game);
                await UpdateClients(id);
            }
        }

        public async Task SuggestedMoveUpdate(string id, int suggestedMoveId)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            game.suggestedMoveId = suggestedMoveId;
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        public async Task AddPlayer(string id, Player player)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.players.Add(player);
            await UpdateClients(id);
        }

        public async Task JoinPlayer(string id, string nickname, string connectionId)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var p = game.players.FirstOrDefault(p => p.nickName == nickname);
            if (p != null)
            {
                p.connectionId = connectionId;
                p.isConnected = true;
                await UpdateClients(id);
            }
        }

        public async Task RemovePlayer(string id, Player player)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var p = game.players.FirstOrDefault(p => p.nickName == player.nickName);
            if (p != null)
            {
                p.isConnected = false;
                await UpdateClients(id);
            }
        }

        public async Task AddWatcher(string id, string watcher)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.watchers.Add(watcher);
            await UpdateClients(id);
        }

        public async Task RemoveWatcher(string id, string watcher)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.watchers.Remove(watcher);
            await UpdateClients(id);
        }

        public async Task PushMove(string id, Move move)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.pauseAgreed || game.state.gameOver)
            {
                return;
            }
            GameHandler.MakeMove(game, move);
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        public async Task PushResign(string id, bool isWhite)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.state.isWhiteResignation = isWhite;
            game.state.playerResigned = true;
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        public async Task PushPauseRequest(string id, string nickname)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.pauseRequests.Contains(nickname))
            {
                game.state.pauseRequests.Remove(nickname);
            }
            else
            {
                game.state.pauseRequests.Add(nickname);
                if ((game.state.pauseRequests.Count >= 2 && !game.settings.isSinglePlayer)
                    || (game.state.pauseRequests.Count >= 1 && game.settings.isSinglePlayer))
                {
                    game.state.pauseRequests = new List<string>();
                    game.state.pauseAgreed = !game.state.pauseAgreed;
                }
            }
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        public async Task PushDrawOffer(string id, string nickname)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.drawOffers.Contains(nickname))
            {
                game.state.drawOffers.Remove(nickname);
            }
            else
            {
                game.state.drawOffers.Add(nickname);
                if (game.state.drawOffers.Count >= 2 && !game.settings.isSinglePlayer)
                {
                    game.state.drawOffers = new List<string>();
                    game.state.drawAgreed = !game.state.drawAgreed;
                }
            }
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        public async Task GameOver(string id, string result, string reason)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var data = new {result = result, reason = reason};
            var json = JsonConvert.SerializeObject(data);
            await hubContext.Clients.Group(id).SendAsync("ReceiveGameOver", json);
        }
    }
}
