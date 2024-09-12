Creating a REST API in your "CheckAndMate" server project and accessing it from your "CheckAndMate.Client" Blazor WebAssembly project involves several steps. Below, I’ll outline the key steps you can take to achieve this:

### Step 1: Setting Up the REST API in the Server Project

1. **Install Required Packages**:
   Ensure that you have the necessary packages installed in your server project. You should have packages like `Microsoft.AspNetCore.Mvc` and `Microsoft.AspNetCore.Http`.

   ```bash
   dotnet add package Microsoft.AspNetCore.Mvc
   dotnet add package Microsoft.AspNetCore.Http
   ```

2. **Configure the Server Project**:
   - Open `Program.cs` and configure your services to add controllers and enable the necessary middleware.

     ```csharp
     var builder = WebApplication.CreateBuilder(args);

     // Add services to the container.
     builder.Services.AddControllers();

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

     app.Run();
     ```

3. **Create a Controller**:
   Create a controller to handle your REST API endpoints. For instance, you can create a `ChessController` to manage chess-related actions.

   ```csharp
   using Microsoft.AspNetCore.Mvc;

   namespace CheckAndMate.Controllers
   {
       [ApiController]
       [Route("api/[controller]")]
       public class ChessController : ControllerBase
       {
           [HttpGet("game/{gameId}")]
           public IActionResult GetGame(string gameId)
           {
               // Replace with your logic to get the game details
               var game = new { GameId = gameId, Status = "ongoing" };
               return Ok(game);
           }

           [HttpPost("move")]
           public IActionResult MakeMove([FromBody] MoveRequest moveRequest)
           {
               // Replace with your logic to handle the move
               var success = true;  // Simulate move processing success
               return Ok(new { Success = success });
           }
       }

       public class MoveRequest
       {
           public string GameId { get; set; }
           public string Move { get; set; }
       }
   }
   ```

### Step 2: Accessing the REST API from the Client Project

1. **Install Required Packages for Blazor WebAssembly**:
   Ensure you have the `Microsoft.AspNetCore.Components.WebAssembly` and `Microsoft.Extensions.Http` packages installed in your `CheckAndMate.Client` project.

   ```bash
   dotnet add package Microsoft.AspNetCore.Components.WebAssembly
   dotnet add package Microsoft.Extensions.Http
   ```

2. **Configure HTTP Client in the Client Project**:
   Open `Program.cs` in your `CheckAndMate.Client` project and configure the `HttpClient`:

   ```csharp
   using Microsoft.AspNetCore.Components.Web;
   using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
   using CheckAndMate.Client;
   using System.Net.Http;

   var builder = WebAssemblyHostBuilder.CreateDefault(args);
   builder.RootComponents.Add<App>("#app");
   builder.RootComponents.Add<HeadOutlet>("head::after");

   builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

   await builder.Build().RunAsync();
   ```

3. **Create a Service to Interact with the API**:
   Create a service class in the client project to handle HTTP requests to your server API.

   ```csharp
   using System.Net.Http;
   using System.Net.Http.Json;
   using System.Threading.Tasks;

   namespace CheckAndMate.Client.Services
   {
       public class ChessService
       {
           private readonly HttpClient _httpClient;

           public ChessService(HttpClient httpClient)
           {
               _httpClient = httpClient;
           }

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
       }

       public class MoveRequest
       {
           public string GameId { get; set; }
           public string Move { get; set; }
       }
   }
   ```

4. **Register the Service in `Program.cs`**:
   Register your `ChessService` in the dependency injection container:

   ```csharp
   builder.Services.AddScoped<ChessService>();
   ```

5. **Use the Service in Your Blazor Components**:
   Inject and use the service in your Blazor components.

   ```razor
   @page "/game/{gameId}"
   @inject CheckAndMate.Client.Services.ChessService ChessService

   @code {
       [Parameter]
       public string GameId { get; set; }

       private object game;

       protected override async Task OnInitializedAsync()
       {
           game = await ChessService.GetGameAsync(GameId);
       }

       private async Task MakeMove()
       {
           var moveRequest = new MoveRequest { GameId = GameId, Move = "e2e4" };
           var success = await ChessService.MakeMoveAsync(moveRequest);
           if (success)
           {
               // Handle successful move
               game = await ChessService.GetGameAsync(GameId);
           }
       }
   }
   ```

This setup provides a basic structure for creating and consuming a REST API in your Blazor WebAssembly application. Adjust the models, logic, and endpoints according to your specific requirements.