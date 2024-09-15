In your SignalR hub, you can use dependency injection to inject and use methods from the service instance, just like you would in a regular ASP.NET Core controller. This setup allows your SignalR hub to leverage the same instance of the `ChessService` that is used across the application.

Here's an example showing how you can reference and use methods from the `ChessService` in your `ChessHub`:

### Step-by-Step Implementation

#### Step 1: Define the `ChessService`

Ensure that your `ChessService` contains the methods you need to call from your SignalR hub.

Here’s a simplified version of `ChessService` including a method to handle moves:

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

        // Method to process a move
        public bool ProcessMove(string gameId, string move)
        {
            if (_games.TryGetValue(gameId, out var gameState))
            {
                // Add the move to the game's move list
                var moves = gameState.Moves.ToList();
                moves.Add(move);
                gameState.Moves = moves.ToArray();
                // Update the game state
                _games[gameId] = gameState;
                return true; // indicating the move was processed successfully
            }
            return false; // indicating the move was not processed
        }

        public IEnumerable<GameState> GetAllGames()
        {
            return _games.Values.ToList();
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

#### Step 2: Inject ChessService into ChessHub

Update your `ChessHub` to use dependency injection and call the methods from `ChessService`.

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
            bool success = _chessService.ProcessMove(gameId, move);
            if (success)
            {
                var gameState = _chessService.GetGameState(gameId);
                // Broadcast the updated game state to all clients in the game group
                await Clients.Group(gameId).SendAsync("ReceiveMove", move);
                await Clients.Group(gameId).SendAsync("ReceiveGameState", gameState);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveError", "Failed to process the move.");
            }
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

#### Step 3: Register ChessService and Configure DI in Program.cs

Ensure the `ChessService` is registered as a singleton service and the `ChessHub` is set up correctly in your `Program.cs`:

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

### Usage Example from a Client:

To see it in action, make sure your Blazor client is properly communicating with the SignalR hub:

**Blazor Component Example**:

```razor
@page "/game/{gameId}"
@inject CheckAndMate.Client.Services.ChessService ChessService

@code {
    [Parameter]
    public string GameId { get; set; }

    private ChessService.GameState gameState;
    private string error;

    protected override async Task OnInitializedAsync()
    {
        gameState = await ChessService.GetGameAsync(GameId); // REST API call for initial data

        ChessService.OnMoveReceived += OnMoveReceived;
        ChessService.OnGameStateUpdated += OnGameStateUpdated;
        ChessService.OnErrorReceived += OnErrorReceived;
        
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

    private void OnErrorReceived(string message)
    {
        error = message;
        StateHasChanged();
    }

    public void Dispose()
    {
        ChessService.OnMoveReceived -= OnMoveReceived;
        ChessService.OnGameStateUpdated -= OnGameStateUpdated;
        ChessService.OnErrorReceived -= OnErrorReceived;
    }
}
```

### Conclusion

By following these steps, your SignalR hub will be able to reference and use methods from the `ChessService` instance. This setup ensures that game state changes like moves are processed consistently via the service layer and broadcasted to all relevant clients in real-time.