using CheckAndMate.Services;
using CheckAndMate.Shared.Chess;
using CheckAndMate.Shared.Utilities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace CheckAndMate.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessService _chessService;
        private readonly ConnectionMappingService _connectionMappingService;

        public ChessHub(ChessService chessService, ConnectionMappingService connectionMappingService)
        {
            _chessService = chessService;
            _connectionMappingService = connectionMappingService;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                _connectionMappingService.AddConnection(Context.ConnectionId, httpContext);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _connectionMappingService.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGameAsPlayer(string gameId)
        {
            var game = _chessService.GetGame(gameId);

            if (game == null)
            {
                return;
            }
            if (game.players.Count >= 2)
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            if (game.players.Count == 0)
            {
                game.players.Add(new Player(Context.ConnectionId, Util.IsPlayerWhite(game, true), true));
            }
            else
            {
                game.players.Add(new Player(Context.ConnectionId, Util.IsPlayerWhite(game, false), false));
            }
            await _chessService.UpdateGame(gameId, game);
        }

        public async Task LeaveGameAsPlayer()
        {
            var game = _chessService.GetAllGames().
                FirstOrDefault(g => g.players.Any(p => p.connectionId == Context.ConnectionId));

            if (game == null)
            {
                return;
            }

            var player = game.players.FirstOrDefault(p => p.connectionId == Context.ConnectionId);
            if (player == null)
            {
                return;
            }
            game.players.Remove(player);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.id);
            await _chessService.UpdateGame(game.id, game);
        }

        public async Task JoinGameAsWatcher(string gameId)
        {
            var game = _chessService.GetGame(gameId);

            if (game == null)
            {
                return;
            }
            if (!game.settings.isWatchable)
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            game.watcherConnections.Add(Context.ConnectionId);
            await _chessService.UpdateGame(gameId, game);
        }

        public async Task LeaveGameAsWatcher()
        {
            var game = _chessService.GetAllGames().
                FirstOrDefault(g => g.watcherConnections.Contains(Context.ConnectionId));

            if (game == null)
            {
                return;
            }

            game.watcherConnections.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.id);
            await _chessService.UpdateGame(game.id, game);
        }

        public async Task SendMove(string moveJson)
        {
            Move? move = JsonConvert.DeserializeObject<Move>(moveJson);
            if (move == null)
            {
                return;
            }

            var game = _chessService.GetAllGames().
                FirstOrDefault(g => g.players.Any(p => p.connectionId == Context.ConnectionId));
            if (game == null)
            {
                return;
            }

            var player = game.players.FirstOrDefault(p => p.connectionId == Context.ConnectionId);
            if (player == null)
            {
                return;
            }
            if (player.isWhite != game.gameState.whiteToMove)
            {
                return;
            }

            bool updated = false;
            var moves = GameHandler.FindValidMoves(game);

            foreach (Move m in moves)
            {
                if (MoveHandler.MovesEqual(m, move))
                {
                    GameHandler.MakeMove(game, m);
                    updated = true;
                }
            }

            if (updated)
            {
                await _chessService.UpdateGame(game.id, game);
            }
        }
    }
}
