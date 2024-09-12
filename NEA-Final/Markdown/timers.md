Implementing timers that are stored in a game object and can be toggled to start or stop decreasing is a common requirement for real-time applications like chess games. Here’s how you can approach this:

1. **Model the Game Timer**: Extend your `GameState` to include timer properties.
2. **Update Timers in Real-Time**: Use background tasks to update the timers.
3. **Manage Timer State**: Allow starting, stopping, and resetting timers.

Below is a comprehensive example demonstrating these concepts.

### Step-by-Step Implementation

#### 1. Extend Your GameState Model

Extend the `GameState` model to include properties for managing timers.

```csharp
namespace CheckAndMate.Services
{
    public class ChessService
    {
        private readonly Dictionary<string, GameState> _games = new Dictionary<string, GameState>();

        public GameState GetGameState(string gameId)
        {
            _games.TryGetValue(gameId, out var gameState);
            return gameState;
        }

        public void UpdateGameState(string gameId, GameState gameState)
        {
            _games[gameId] = gameState;
        }

        public class GameState
        {
            public string GameId { get; set; }
            public string[] Moves { get; set; }
            public int Player1Time { get; set; }  // Time in seconds
            public int Player2Time { get; set; }  // Time in seconds
            public bool Player1TimerRunning { get; set; }
            public bool Player2TimerRunning { get; set; }
        }
    }
}
```

#### 2. Manage Timer Updates in Real-Time

Create a background task that periodically updates the timers.

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

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

    private void UpdateTimers(object state)
    {
        foreach (var game in _chessService.GetAllGames())
        {
            if (game.Player1TimerRunning)
            {
                game.Player1Time -= 1;
            }

            if (game.Player2TimerRunning)
            {
                game.Player2Time -= 1;
            }

            _chessService.UpdateGameState(game.GameId, game);
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
```

#### 3. Register TimerService in DI

Register the `TimerService` in your `Program.cs` file.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChessService>(); // Register the ChessService as a singleton
builder.Services.AddHostedService<TimerService>(); // Register the TimerService

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChessHub>("/chesshub");

app.Run();
```

#### 4. Update SignalR Hub and Controller to Manage Timer State

Update your SignalR hub and REST controller to start, stop, and reset the timers.

**SignalR Hub**:
```csharp
public class ChessHub : Hub
{
    private readonly ChessService _chessService;

    public ChessHub(ChessService chessService)
    {
        _chessService = chessService;
    }

    public async Task ToggleTimer(string gameId, string player)
    {
        var gameState = _chessService.GetGameState(gameId);
        if (gameState != null)
        {
            if (player == "Player1")
            {
                gameState.Player1TimerRunning = !gameState.Player1TimerRunning;
            }
            else if (player == "Player2")
            {
                gameState.Player2TimerRunning = !gameState.Player2TimerRunning;
            }
            _chessService.UpdateGameState(gameId, gameState);
        }

        await Clients.Group(gameId).SendAsync("TimerToggled", gameState);
    }
}
```

**REST Controller**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ChessController : ControllerBase
{
    private readonly ChessService _chessService;

    public ChessController(ChessService chessService)
    {
        _chessService = chessService;
    }

    [HttpGet("game/{gameId}")]
    public IActionResult GetGame(string gameId)
    {
        var gameState = _chessService.GetGameState(gameId);
        if (gameState == null)
        {
            return NotFound();
        }

        return Ok(gameState);
    }

    [HttpPost("move")]
    public IActionResult MakeMove([FromBody] MoveRequest moveRequest)
    {
        var gameState = _chessService.GetGameState(moveRequest.GameId);
        if (gameState == null)
        {
            return BadRequest("Game not found");
        }

        var moves = gameState.Moves.ToList();
        moves.Add(moveRequest.Move);
        gameState.Moves = moves.ToArray();
        _chessService.UpdateGameState(moveRequest.GameId, gameState);

        return Ok(new { Success = true });
    }

    [HttpPost("toggle-timer")]
    public IActionResult ToggleTimer([FromBody] TimerToggleRequest timerToggleRequest)
    {
        var gameState = _chessService.GetGameState(timerToggleRequest.GameId);
        if (gameState == null)
        {
            return BadRequest("Game not found");
        }

        if (timerToggleRequest.Player == "Player1")
        {
            gameState.Player1TimerRunning = !gameState.Player1TimerRunning;
        }
        else if (timerToggleRequest.Player == "Player2")
        {
            gameState.Player2TimerRunning = !gameState.Player2TimerRunning;
        }

        _chessService.UpdateGameState(timerToggleRequest.GameId, gameState);
        return Ok(new { Success = true });
    }
}

public class TimerToggleRequest
{
    public string GameId { get; set; }
    public string Player { get; set; }
}
```

### Conclusion

By implementing the above steps, you can create a robust system to manage timers in your chess game. The game state, including timers, is updated in real-time using `TimerService`, which periodically decreases timer values. Both the SignalR hub and REST API controller can start, stop, and manage the timers via shared `ChessService`. This setup ensures that the game timers are accurately synchronized across all clients and managed efficiently server-side.