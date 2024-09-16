using CheckAndMate.Services;
using CheckAndMate.Shared.Chess;
using Microsoft.AspNetCore.SignalR;

namespace CheckAndMate.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessService _chessService;

        public ChessHub(ChessService chessService)
        {
            _chessService = chessService;
        }

        public async Task JoinGameAsPlayer(string gameId)
        {
            var game = _chessService.GetGame(gameId);
            
            if (game == null)
            {
                return;
            }
            if (game.playerConnections.Count >= 2)
            {
                return;
            }
            
            game.playerConnections.Add(Context.ConnectionId);
            _chessService.UpdateGame(gameId, game);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task LeaveGameAsPlayer()
        {
            var game = _chessService.GetAllGames().
                FirstOrDefault(g => g.playerConnections.Contains(Context.ConnectionId));

            if (game == null)
            {
                return;
            }

            game.playerConnections.Remove(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.id);
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

            game.watcherConnections.Add(Context.ConnectionId);
            _chessService.UpdateGame(gameId, game);

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
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
        }

        public async Task SendMove(Move move)
        {
            var game = _chessService.GetAllGames().
                FirstOrDefault(g => g.playerConnections.Contains(Context.ConnectionId));

            if (game == null)
            {
                return;
            }

            bool updated = false;

            var moves = GameHandler.FindValidMoves(game);

            foreach (Move m in moves)
            {
                if (MoveHandler.Equals(m, move))
                {
                    GameHandler.MakeMove(game, m);
                    updated = true;
                }
            }

            if (updated)
            {
                _chessService.UpdateGame(game.id, game);
            }
        }
    }
}
