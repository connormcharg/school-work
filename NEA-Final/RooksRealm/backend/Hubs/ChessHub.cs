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
                throw new HubException("InvalidGameId: No game with that gameId found");
            }

            var nickname = userService.GetNickname(Context.ConnectionId);

            if (nickname == null)
            {
                throw new HubException("No nickname found for the given connection id");
            }
            
            int rating = userService.GetRating(Context.ConnectionId);

            if (game.players.Count >= 2)
            {
                if (game.players.Any(p => p.nickName == nickname))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                    await chessService.JoinPlayer(gameId, nickname, Context.ConnectionId);
                }
                else
                {
                    throw new HubException("Game already full and you are not in the players list");
                }
            }
            else
            {
                if (game.players.Count == 0)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                    await chessService.AddPlayer(game.id, new Player(Context.ConnectionId, GameUtilities.IsPlayerWhite(game, true), true, nickname, false, rating));
                    await chessService.JoinPlayer(game.id, nickname, Context.ConnectionId);
                }
                else if (game.settings.isSinglePlayer)
                {
                    if (game.players.Any(p => p.nickName == nickname))
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                        await chessService.JoinPlayer(gameId, nickname, Context.ConnectionId);
                    }
                    else
                    {
                        throw new HubException("Game already full and you are not in the players list");
                    }
                }
                else
                {
                    if (game.players.Any(p => p.nickName == nickname))
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                        await chessService.JoinPlayer(game.id, nickname, Context.ConnectionId);
                    }
                    else
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                        await chessService.AddPlayer(game.id, new Player(Context.ConnectionId, GameUtilities.IsPlayerWhite(game, false), false, nickname, false, rating));
                        await chessService.JoinPlayer(game.id, nickname, Context.ConnectionId);
                    }
                }
            }
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

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.id);
            await chessService.RemovePlayer(game.id, player);
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
            await chessService.AddWatcher(game.id, Context.ConnectionId);
        }

        public async Task LeaveGameAsWatcher()
        {
            var game = chessService.GetAllGames().
                FirstOrDefault(g => g.watchers.Contains(Context.ConnectionId));

            if (game == null)
            {
                return;
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.id);
            await chessService.RemoveWatcher(game.id, Context.ConnectionId);
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
                    await chessService.PushMove(game.id, m);
                }
            }
        }

        public async Task SendResignation()
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

            await chessService.PushResign(game.id, player.isWhite);
        }

        public async Task SendPauseRequest()
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

            await chessService.PushPauseRequest(game.id, player.nickName);
        }

        public async Task SendDrawOffer()
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

            await chessService.PushDrawOffer(game.id, player.nickName);
        }
    }
}
