namespace backend.Services
{
    public class TimerService : IHostedService, IDisposable
    {
        private Timer? timer;
        private readonly ChessService chessService;

        public TimerService(ChessService chessService)
        {
            this.chessService = chessService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(UpdateTimers, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async void UpdateTimers(object? state)
        {
            foreach (var game in chessService.GetAllGames())
            {
                if (!game.settings.isTimed)
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

                if (!(game.state.whiteTimeRunning || game.state.blackTimeRunning))
                {
                    if (game.settings.isSinglePlayer)
                    {
                        if (game.players.Count == 1)
                        {
                            await chessService.StartTimer(game.id, true);
                        }
                    }
                    else
                    {
                        if (game.players.Count == 2)
                        {
                            await chessService.StartTimer(game.id, true);
                        }
                    }
                }

                await chessService.UpdateTimers(game.id);
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