namespace backend.Services
{
    using backend.Classes.Engine;
    using backend.Classes.Handlers;
    using Microsoft.AspNetCore.Mvc.Formatters;

    /// <summary>
    /// Defines the <see cref="EngineService" />
    /// </summary>
    public class EngineService : IHostedService, IDisposable
    {
        /// <summary>
        /// Defines the _timer
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Defines the _chessService
        /// </summary>
        private readonly ChessService _chessService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineService"/> class.
        /// </summary>
        /// <param name="chessService">The chessService<see cref="ChessService"/></param>
        public EngineService(ChessService chessService)
        {
            _chessService = chessService;
        }

        /// <summary>
        /// The StartAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(FindEngineMoves, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
            return Task.CompletedTask;
        }

        /// <summary>
        /// The FindEngineMoves
        /// </summary>
        /// <param name="state">The state<see cref="object"/></param>
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

        /// <summary>
        /// The StopAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
