using CheckAndMate.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CheckAndMate.Services
{
    public class TimerService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ChessService _chessService;

        public TimerService(ChessService chessService)
        {
            _chessService = chessService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateTimers, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async void UpdateTimers(object state)
        {
            foreach (var game in _chessService.GetAllGames())
            {
                var updated = false;

                if (game.gameState.whiteTimeRunning)
                {
                    game.gameState.whiteTime -= 1;
                    updated = true;
                }

                if (game.gameState.blackTimeRunning)
                {
                    game.gameState.blackTime -= 1;
                    updated = true;
                }

                if (updated)
                {
                    await _chessService.UpdateGame(game.id, game);
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
