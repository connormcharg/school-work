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
                var updated = false;

                if (game.state.whiteTimeRunning)
                {
                    game.state.whiteTime -= 1;
                    updated = true;
                }

                if (game.state.blackTimeRunning)
                {
                    game.state.blackTime -= 1;
                    updated = true;
                }

                if (updated)
                {
                    await chessService.UpdateGame(game.id, game);
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
