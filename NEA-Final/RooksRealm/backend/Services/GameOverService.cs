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
            timer = new Timer(CheckGames, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));
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
                            await chessService.GameOver(game.id, "Checkmate!", "Black won by checkmate!");
                        }
                        else
                        {
                            await chessService.GameOver(game.id, "Checkmate!", "White won by checkmate!");
                        }
                    }
                    else if (game.state.whiteTime <= 0)
                    {
                        await chessService.GameOver(game.id, "Black Wins!", "White ran out of time!");
                    }
                    else if (game.state.blackTime <= 0)
                    {
                        await chessService.GameOver(game.id, "White Wins!", "Black ran out of time!");
                    }
                    else if (game.state.staleMate)
                    {
                        await chessService.GameOver(game.id, "Stalemate!", "No more possible moves!");
                    }
                    else if (game.state.fiftyMoveCounter >= 100)
                    {
                        await chessService.GameOver(game.id, "Draw!", "Fifty moves without moving a pawn or capturing a piece!");
                    }
                    else if (game.state.drawAgreed)
                    {
                        await chessService.GameOver(game.id, "Draw!", "Players agreed to a draw!");
                    }
                    else if (game.state.playerResigned)
                    {
                        if (game.state.isWhiteResignation)
                        {
                            await chessService.GameOver(game.id, "Black wins!", "Black won by white's resignation!");
                        }
                        else
                        {
                            await chessService.GameOver(game.id, "White wins!", "White won by black's resignation!");
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
