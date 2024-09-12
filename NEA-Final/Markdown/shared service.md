To enable communication between your SignalR hub and REST API controllers on the server side, you can leverage shared services using Dependency Injection (DI). By injecting common services into both the SignalR hub and the REST API controllers, you can ensure they both access and manipulate the same state or perform actions on shared resources.

Here's how you can achieve this:

### Step-by-Step Implementation

#### 1. Define a Shared Service

First, define a service that will manage shared state or logic. For instance, you can create a `ChessService` that handles game logic, state, and communication.

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
            // Add other game-related properties
        }
    }
}
```

#### 2. Register the Shared Service in DI

Register the `ChessService` in your DI container in `Program.cs` of the server project.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ChessService>();  // Register the ChessService as a singleton

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

#### 3. Inject the Shared Service into the SignalR Hub

Modify your `ChessHub` class to inject and use `ChessService`.

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
            // Update game state using the shared service
            var gameState = _chessService.GetGameState(gameId);
            if (gameState != null)
            {
                var moves = gameState.Moves.ToList();
                moves.Add(move);
                gameState.Moves = moves.ToArray();
                _chessService.UpdateGameState(gameId, gameState);
            }
            
            // Broadcast the move to all connected clients
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

#### 4. Inject the Shared Service into REST API Controllers

Modify your REST API controller to inject and use `ChessService`.

```csharp
using Microsoft.AspNetCore.Mvc;
using CheckAndMate.Services;

namespace CheckAndMate.Controllers
{
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
            // Get game details from the shared service
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
            // Update game state using the shared service
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
    }

    public class MoveRequest
    {
        public string GameId { get; set; }
        public string Move { get; set; }
    }
}
```

### Conclusion

By using a shared service and DI, you can facilitate communication and shared state between your SignalR hub and REST API controllers. This approach ensures that both parts of your application are working with the same data and business logic, making it easier to maintain consistency and handle complex interactions in your chess game.