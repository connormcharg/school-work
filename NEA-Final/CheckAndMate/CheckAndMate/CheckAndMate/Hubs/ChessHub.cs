using CheckAndMate.Services;
using CheckAndMate.Shared.Chess;
using CheckAndMate.Shared.Utilities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace CheckAndMate.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessService _chessService;
        private readonly ConnectionMappingService _connectionMappingService;
        private readonly UserService _userService;

        public ChessHub(ChessService chessService, ConnectionMappingService connectionMappingService, 
            UserService userService)
        {
            _chessService = chessService;
            _connectionMappingService = connectionMappingService;
            _userService = userService;
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

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var game = _chessService.GetAllGames().
                FirstOrDefault(g => g.watcherConnections.Contains(Context.ConnectionId) || g.players.Any(p =>
                {
                    return p.connectionId == Context.ConnectionId;
                }));

            if (game == null)
            {
                return;
            }

            if (game.watcherConnections.Contains(Context.ConnectionId))
            {
                await LeaveGameAsWatcher();
            }
            else if (game.players.Any(p => p.connectionId == Context.ConnectionId))
            {
                await LeaveGameAsPlayer();
            }

            _connectionMappingService.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGameAsPlayer(string gameId)
        {
            var game = _chessService.GetGame(gameId);

            if (game == null)
            {
                return;
            }

            var nickname = await _userService.GetNicknameAsync(Context.ConnectionId);
            
            if (nickname == null)
            {
                return;
            }

            if (game.players.Count >= 2)
            {
                if (game.players.Any(p => p.nickName == nickname))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (game.players.Count == 0)
                {
                    game.players.Add(new Player(Context.ConnectionId, Util.IsPlayerWhite(game, true), true, nickname));
                }
                else if (game.settings.isSinglePlayer)
                {
                    return;
                }
                else
                {
                    game.players.Add(new Player(Context.ConnectionId, Util.IsPlayerWhite(game, false), false, nickname));
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
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

            var moves = GameHandler.FindValidMoves(game);

            foreach (Move m in moves)
            {
                if (MoveHandler.MovesEqual(m, move))
                {
                    GameHandler.MakeMove(game, m);
                }
            }

            await _chessService.UpdateGame(game.id, game);
        }
    }
}
