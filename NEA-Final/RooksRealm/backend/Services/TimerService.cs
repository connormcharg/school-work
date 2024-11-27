namespace backend.Services
{
    /// <summary>
    /// Defines the <see cref="TimerService" />
    /// </summary>
    public class TimerService : IHostedService, IDisposable
    {
        /// <summary>
        /// Defines the timer
        /// </summary>
        private Timer? timer;

        /// <summary>
        /// Defines the chessService
        /// </summary>
        private readonly ChessService chessService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerService"/> class.
        /// </summary>
        /// <param name="chessService">The chessService<see cref="ChessService"/></param>
        public TimerService(ChessService chessService)
        {
            this.chessService = chessService;
        }

        /// <summary>
        /// The StartAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(UpdateTimers, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        /// <summary>
        /// The UpdateTimers
        /// </summary>
        /// <param name="state">The state<see cref="object?"/></param>
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

        /// <summary>
        /// The StopAsync
        /// </summary>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
