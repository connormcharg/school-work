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

        public async Task SendGame(string gameId, Game game)
        {
            await Clients.Group(gameId).SendAsync("ReceiveGame", game);
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task SendMove(string gameId, Move move)
        {
            var game = _chessService.GetGame(gameId);
            if (game == null)
            {
                return;
            }

            var validMoves = GameHandler.FindValidMoves(game);

            if (validMoves == null)
            {
                return;
            }

            var valid = false;
            foreach(var m in validMoves)
            {
                if (MoveHandler.MovesEqual(m, move))
                {
                    valid = true;
                    GameHandler.MakeMove(game, m);
                }
            }

            if (valid)
            {
                _chessService.UpdateGame(gameId, game);
            }
        }
    }
}
