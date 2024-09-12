Yes, it is indeed possible and often practical to combine both a REST API and SignalR in your application. Each technology has its strengths, and using them together allows you to leverage these strengths effectively:

- **REST API**: Best suited for CRUD operations that don't require real-time updates, such as fetching game details, user profiles, historical data, etc.
- **SignalR**: Ideal for real-time communication, such as updating the game board in real time, synchronizing game states, or broadcasting changes to all connected clients.

This combination allows you to efficiently manage both single-use requests and real-time communication.

### Step-by-Step Implementation

#### 1. Setting Up REST API in the Server Project (CheckAndMate)

**Ensure you have the necessary REST API setup as described previously**. For the server part, we’ll only need to add SignalR.

#### 2. Setting Up SignalR in the Server Project (CheckAndMate)

1. **Install SignalR Package**:
   Make sure you have `Microsoft.AspNetCore.SignalR` installed.

   ```bash
   dotnet add package Microsoft.AspNetCore.SignalR
   ```

2. **Add SignalR to the Project (Program.cs)**:

   ```csharp
   var builder = WebApplication.CreateBuilder(args);

   // Add services to the container.
   builder.Services.AddControllers();
   builder.Services.AddSignalR();  // Adding SignalR

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
   app.MapHub<ChessHub>("/chesshub");  // Mapping the SignalR Hub

   app.Run();
   ```

3. **Create a SignalR Hub**:
   Create your SignalR Hub, e.g., `ChessHub`:

   ```csharp
   using Microsoft.AspNetCore.SignalR;
   using System.Threading.Tasks;

   namespace CheckAndMate.Hubs
   {
       public class ChessHub : Hub
       {
           public async Task SendMove(string gameId, string move)
           {
               // Broadcast the move to all connected clients in the specified group
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

#### 3. Configuring Client for Both REST and SignalR (CheckAndMate.Client)

1. **Install Required Packages**:
   Ensure you have the necessary packages:

   ```bash
   dotnet add package Microsoft.AspNetCore.Components.WebAssembly
   dotnet add package Microsoft.Extensions.Http
   dotnet add package Microsoft.AspNetCore.SignalR.Client
   ```

2. **Configure HTTP Client in `Program.cs`**:

   ```csharp
   using Microsoft.AspNetCore.Components.Web;
   using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
   using CheckAndMate.Client;
   using Microsoft.AspNetCore.SignalR.Client;
   using System.Net.Http;

   var builder = WebAssemblyHostBuilder.CreateDefault(args);
   builder.RootComponents.Add<App>("#app");
   builder.RootComponents.Add<HeadOutlet>("head::after");

   // Register HttpClient for REST API
   builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

   // Register SignalR Hub Connection and Service
   builder.Services.AddSingleton(sp => 
   {
       var navigationManager = sp.GetRequiredService<NavigationManager>();
       return new HubConnectionBuilder()
           .WithUrl(navigationManager.ToAbsoluteUri("/chesshub"))
           .Build();
   });
   builder.Services.AddSingleton<ChessService>();

   await builder.Build().RunAsync();
   ```

3. **Create REST API and SignalR Service**:
   Combine both REST API and SignalR in a service:

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

           // REST API methods
           public async Task<object> GetGameAsync(string gameId)
           {
               return await _httpClient.GetFromJsonAsync<object>($"api/chess/game/{gameId}");
           }

           public async Task<bool> MakeMoveAsync(MoveRequest moveRequest)
           {
               var response = await _httpClient.PostAsJsonAsync("api/chess/move", moveRequest);
               response.EnsureSuccessStatusCode();
               var result = await response.Content.ReadFromJsonAsync<dynamic>();
               return result.Success;
           }

           // SignalR methods
           public async Task StartSignalRAsync()
           {
               _hubConnection.On<string>("ReceiveMove", move =>
               {
                   OnMoveReceived?.Invoke(move);
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
       }

       public class MoveRequest
       {
           public string GameId { get; set; }
           public string Move { get; set; }
       }
   }
   ```

4. **Use the Combined Service in Blazor Components**:

   ```razor
   @page "/game/{gameId}"
   @inject CheckAndMate.Client.Services.ChessService ChessService

   @code {
       [Parameter]
       public string GameId { get; set; }

       private object game;
       private string move;

       protected override async Task OnInitializedAsync()
       {
           game = await ChessService.GetGameAsync(GameId); // REST API call for initial data

           ChessService.OnMoveReceived += OnMoveReceived;
           await ChessService.StartSignalRAsync();  // SignalR connection

           await ChessService.JoinGameAsync(GameId);  // Join SignalR group
       }

       private void OnMoveReceived(string move)
       {
           // Update game board with the move
           this.move = move;
           StateHasChanged();
       }

       private async Task MakeMove()
       {
           string newMove = "e2e4";  // Capture the actual move value
           await ChessService.SendMoveAsync(GameId, newMove);  // SignalR call for real-time communication

           // Optionally, also update via REST API for persistence
           var moveRequest = new MoveRequest { GameId = GameId, Move = newMove };
           await ChessService.MakeMoveAsync(moveRequest);
       }

       public void Dispose()
       {
           ChessService.OnMoveReceived -= OnMoveReceived;
       }
   }
   ```

### Conclusion

Combining both REST API and SignalR in your Blazor application allows you to handle both single-use requests and real-time updates efficiently. REST API can be used for initial data fetching and other non-real-time operations, while SignalR can handle real-time game updates and interactions, providing a seamless and responsive user experience.