namespace backend.Services
{
    public class GameOverService : IHostedService, IDisposable
    {
        private Timer? timer;
        private readonly ChessService chessService;

        public GameOverService(ChessService chessService)
        {
            this.chessService = chessService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(CheckGames, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.1));
            return Task.CompletedTask;
        }

        private async void CheckGames(object state)
        {
            foreach (var game in chessService.GetAllGames())
            {
                if (game.state.gameOver)
                {
                    if (game.state.checkMate)
                    {
                        if (game.state.whiteToMove)
                        {
                            await chessService.GameOver(game.id, "checkmate", "black won");
                        }
                        else
                        {
                            await chessService.GameOver(game.id, "checkmate", "white won");
                        }
                    }
                    else if (game.state.whiteTime <= 0)
                    {
                        await chessService.GameOver(game.id, "timeranout", "black won");
                    }
                    else if (game.state.blackTime <= 0)
                    {
                        await chessService.GameOver(game.id, "timeranout", "white won");
                    }
                    else if (game.state.staleMate)
                    {
                        await chessService.GameOver(game.id, "stalemate", "no possible moves");
                    }
                    else if (game.state.fiftyMoveCounter >= 100)
                    {
                        await chessService.GameOver(game.id, "draw", "fifty move rule");
                    }
                    else if (game.state.drawAgreed)
                    {
                        await chessService.GameOver(game.id, "draw", "players agreed to a draw");
                    }
                    else if (game.state.playerResigned)
                    {
                        if (game.state.isWhiteResignation)
                        {
                            await chessService.GameOver(game.id, "resignation", "black won");
                        }
                        else
                        {
                            await chessService.GameOver(game.id, "resignation", "white won");
                        }
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
