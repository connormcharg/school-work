To send the updated game state timers to all connections in a game every time the timer ticks, you can leverage SignalR's capabilities to broadcast messages to clients connected to a specific group. Here's how you can accomplish that:

### Step-by-Step Implementation

1. **Update TimerService to Use SignalR for Broadcasting**: Modify the `TimerService` to include SignalR hub context to broadcast updates.
2. **Set Up SignalR Hub for Broadcasting**: Ensure the SignalR hub can send messages to all clients.

### 1. Inject SignalR HubContext into TimerService

First, we need to inject `IHubContext` into the `TimerService` so that it can broadcast updated game state to connected clients.

**Modified `TimerService`**:

```csharp
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CheckAndMate.Hubs;

public class TimerService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly ChessService _chessService;
    private readonly IHubContext<ChessHub> _hubContext;

    public TimerService(ChessService chessService, IHubContext<ChessHub> hubContext)
    {
        _chessService = chessService;
        _hubContext = hubContext;
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

            if (game.Player1TimerRunning)
            {
                game.Player1Time -= 1;
                updated = true;
            }

            if (game.Player2TimerRunning)
            {
                game.Player2Time -= 1;
                updated = true;
            }

            if (updated)
            {
                _chessService.UpdateGameState(game.GameId, game);

                // Broadcast the updated game state to all clients in the game's group
                await _hubContext.Clients.Group(game.GameId).SendAsync("ReceiveGameState", game);
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
```

### 2. Register TimerService with DI in `Program.cs`

Update the `Program.cs` file to register `TimerService` and ensure it’s properly initialized.

```csharp
using CheckAndMate.Hubs;
using CheckAndMate.Services;

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

### 3. Set Up SignalR Hub for Broadcasting

Ensure your `ChessHub` is set up to handle clients receiving the updated game state.

**Modified `ChessHub`**:

```csharp
using Microsoft.AspNetCore.SignalR;
using CheckAndMate.Services;
using System.Threading.Tasks;

namespace CheckAndMate.Hubs
{
    public class ChessHub : Hub
    {
        private readonly ChessService _chessService;

        public ChessHub(ChessService chessService)
        {
            _chessService = chessService;
        }

        public async Task SendMove(string gameId, string move)
        {
            var gameState = _chessService.GetGameState(gameId);
            if (gameState != null)
            {
                var moves = gameState.Moves.ToList();
                moves.Add(move);
                gameState.Moves = moves.ToArray();
                _chessService.UpdateGameState(gameId, gameState);
            }

            await Clients.Group(gameId).SendAsync("ReceiveMove", move);
        }

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
```

### 4. Update Client to Handle Real-Time Updates

Ensure your Blazor components handle real-time updates from SignalR.

**Blazor Component Example**:

```razor
@page "/game/{gameId}"
@inject CheckAndMate.Client.Services.ChessService ChessService

@code {
    [Parameter]
    public string GameId { get; set; }

    private ChessService.GameState gameState;

    protected override async Task OnInitializedAsync()
    {
        gameState = await ChessService.GetGameAsync(GameId); // REST API call for initial data

        ChessService.OnMoveReceived += OnMoveReceived;
        ChessService.OnGameStateUpdated += OnGameStateUpdated;
        
        await ChessService.StartSignalRAsync();  // SignalR connection
        await ChessService.JoinGameAsync(GameId);  // Join SignalR group
    }

    private void OnMoveReceived(string move)
    {
        // Update game state with the move
        gameState.Moves = gameState.Moves.Append(move).ToArray();
        StateHasChanged();
    }

    private void OnGameStateUpdated(ChessService.GameState updatedGameState)
    {
        // Update game state from received state
        gameState = updatedGameState;
        StateHasChanged();
    }

    public void Dispose()
    {
        ChessService.OnMoveReceived -= OnMoveReceived;
        ChessService.OnGameStateUpdated -= OnGameStateUpdated;
    }
}
```

**Client-Side ChessService**:

```csharp
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CheckAndMate.Client.Services
{
    public class ChessService
    {
        private readonly HttpClient _httpClient;
        private readonly HubConnection _hubConnection;

        public ChessService(HttpClient httpClient, HubConnection hubConnection)
        {
            _httpClient = httpClient;
            _hubConnection = hubConnection;
        }

        public async Task<GameState> GetGameAsync(string gameId)
        {
            return await _httpClient.GetFromJsonAsync<GameState>($"api/chess/game/{gameId}");
        }

        public async Task<bool> MakeMoveAsync(MoveRequest moveRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/chess/move", moveRequest);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            return result.Success;
        }

        public async Task StartSignalRAsync()
        {
            _hubConnection.On<string>("ReceiveMove", move =>
            {
                OnMoveReceived?.Invoke(move);
            });

            _hubConnection.On<GameState>("ReceiveGameState", gameState =>
            {
                OnGameStateUpdated?.Invoke(gameState);
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMoveAsync(string gameId, string move)
        {
            await _hubConnection.SendAsync("SendMove", gameId, move);
        }

        public async Task JoinGameAsync(string gameId)
        {
            await _hubConnection.SendAsync("JoinGame", gameId);
        }

        public async Task LeaveGameAsync(string gameId)
        {
            await _hubConnection.SendAsync("LeaveGame", gameId);
        }

        public event Action<string> OnMoveReceived;
        public event Action<GameState> OnGameStateUpdated;

        public class GameState
        {
            public string GameId { get; set; }
            public string[] Moves { get; set; }
            public int Player1Time { get; set; }  // Time in seconds
            public int Player2Time { get; set; }  // Time in seconds
            public bool Player1TimerRunning { get; set; }
            public bool Player2TimerRunning { get; set; }
        }

        public class MoveRequest
        {
            public string GameId { get; set; }
            public string Move { get; set; }
        }
    }
}
```

### Conclusion

By following these steps, you can ensure that your game state, including timers, is broadcast to all connected clients in real-time via SignalR. The `TimerService` periodically updates the game state and then uses SignalR to notify all clients in the relevant game group. This approach ensures that all players have a consistent and up-to-date view of the game.