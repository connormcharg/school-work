using backend.Classes.Handlers;
using backend.Classes.State;
using backend.Classes.Utilities;
using backend.Services;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace backend.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessService chessService;
        private readonly ConnectionMappingService connectionMappingService;
        private readonly UserService userService;

        public ChessHub(ChessService chessService, 
            ConnectionMappingService connectionMappingService, 
            UserService userService)
        {
            this.chessService = chessService;
            this.connectionMappingService = connectionMappingService;
            this.userService = userService;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                connectionMappingService.AddConnection(Context.ConnectionId, httpContext);
            }
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var game = chessService.GetAllGames().
                FirstOrDefault(g => g.watchers.Contains(Context.ConnectionId) || g.players.Any(
                    p => p.connectionId == Context.ConnectionId));

            if (game != null)
            {
                if (game.watchers.Contains(Context.ConnectionId))
                {
                    await LeaveGameAsWatcher();
                }
                else if (game.players.Any(p => p.connectionId == Context.ConnectionId))
                {
                    await LeaveGameAsPlayer();
                }
            }

            connectionMappingService.RemoveConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGameAsPlayer(string gameId)
        {
            var game = chessService.GetGame(gameId);

            if (game == null)
            {
                return;
            }

            var nickname = userService.GetNickname(Context.ConnectionId);

            if (nickname == null)
            {
                return;
            }

            if (game.players.Count >= 2)
            {
                Console.WriteLine("players >= 2");
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
                    game.players.Add(new Player(Context.ConnectionId, GameUtilities.IsPlayerWhite(game, true), true, nickname));
                }
                else if (game.settings.isSinglePlayer)
                {
                    return;
                }
                else
                {
                    game.players.Add(new Player(Context.ConnectionId, GameUtilities.IsPlayerWhite(game, false), false, nickname));
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            }

            await chessService.UpdateGame(gameId, game);
        }

        public async Task LeaveGameAsPlayer()
        {
            var game = chessService.GetAllGames().
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
            await chessService.UpdateGame(game.id, game);
        }

        public async Task JoinGameAsWatcher(string gameId)
        {
            var game = chessService.GetGame(gameId);

            if (game == null)
            {
                return;
            }
            if (!game.settings.isWatchable)
            {
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            game.watchers.Add(Context.ConnectionId);
            await chessService.UpdateGame(gameId, game);
        }

        public async Task LeaveGameAsWatcher()
        {
            var game = chessService.GetAllGames().
                FirstOrDefault(g => g.watchers.Contains(Context.ConnectionId));

            if (game == null)
            {
                return;
            }

            game.watchers.Remove(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.id);
            await chessService.UpdateGame(game.id, game);
        }

        public async Task SendMove(string moveJson)
        {
            Move? move = JsonConvert.DeserializeObject<Move>(moveJson);
            if (move == null)
            {
                return;
            }

            var game = chessService.GetAllGames().
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
            if (player.isWhite != game.state.whiteToMove)
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

            await chessService.UpdateGame(game.id, game);
        }
    }
}
