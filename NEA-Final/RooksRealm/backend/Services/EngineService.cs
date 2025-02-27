namespace backend.Services
{
    using backend.Classes.Engine;
    using backend.Classes.Handlers;

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
            _timer = new Timer(FindEngineMoves, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
            return Task.CompletedTask;
        }

        private async void FindEngineMoves(object state)
        {
            foreach (var game in _chessService.GetAllGames())
            {
                if (game.players.Count == 0)
                {
                    continue;
                }
                if (!game.settings.isSinglePlayer)
                {
                    continue;
                }
                if (game.state.checkMate || game.state.staleMate)
                {
                    continue;
                }
                if (game.state.gameOver)
                {
                    continue;
                }
                if (game.state.pauseAgreed)
                {
                    continue;
                }

                if (game.state.whiteToMove == game.players[0].isWhite)
                { // player's turn
                    var engine = new MinMaxEngine();
                    game.currentValidMoves = GameHandler.FindValidMoves(game);
                    engine.FindBestMove(game, game.currentValidMoves);
                    if (engine.nextMove == null)
                    {
                        engine.FindRandomMove(game, game.currentValidMoves);
                    }
                    await _chessService.SuggestedMoveUpdate(game.id, engine.nextMove.moveID);
                }
                else
                { // ai's turn
                    var engine = new MinMaxEngine();
                    game.currentValidMoves = GameHandler.FindValidMoves(game);
                    engine.FindBestMove(game, game.currentValidMoves);
                    if (engine.nextMove == null)
                    {
                        game.currentValidMoves = GameHandler.FindValidMoves(game);
                        engine.FindRandomMove(game, game.currentValidMoves);
                    }

                    GameHandler.MakeMove(game, engine.nextMove);
                    int engineMoveId = engine.nextMove.moveID;

                    if (game.state.checkMate || game.state.staleMate)
                    {
                        await _chessService.EngineUpdate(game.id, engineMoveId, 0);
                        continue;
                    }

                    game.currentValidMoves = GameHandler.FindValidMoves(game);
                    engine.FindBestMove(game, game.currentValidMoves);
                    if (engine.nextMove == null)
                    {
                        game.currentValidMoves = GameHandler.FindValidMoves(game);
                        engine.FindRandomMove(game, game.currentValidMoves);
                    }
                    game.suggestedMoveId = engine.nextMove.moveID;

                    await _chessService.EngineUpdate(game.id, engineMoveId, game.suggestedMoveId);
                }
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