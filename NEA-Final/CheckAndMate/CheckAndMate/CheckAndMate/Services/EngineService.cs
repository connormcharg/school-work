using CheckAndMate.Shared.Chess;

namespace CheckAndMate.Services
{
    public class EngineService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ChessService _chessService;

        public EngineService(ChessService chessService)
        {
            _chessService = chessService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(FindEngineMoves, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private async void FindEngineMoves(object state)
        {
            foreach (var game in _chessService.GetAllGames())
            {
                if (!game.settings.isSinglePlayer)
                {
                    continue;
                }
                if (game.gameState.whiteToMove == game.players[0].isWhite)
                {
                    continue;
                }

                var engine = new ChessEngine();
                game.currentValidMoves = GameHandler.FindValidMoves(game);
                engine.FindBestMove(game, game.currentValidMoves);
                if (engine.nextMove == null)
                {
                    engine.FindRandomMove(game, game.currentValidMoves);
                }

                GameHandler.MakeMove(game, engine.nextMove);
                await _chessService.UpdateGame(game.id, game);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
