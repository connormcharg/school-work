using CheckAndMate.Client;
using CheckAndMate.Client.Services;
using CheckAndMate.Shared.Chess;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

namespace CheckAndMate.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HubConnectionBuilder()
                    .WithUrl(navigationManager.ToAbsoluteUri("/chesshub"))
                    .ConfigureLogging(logging =>
                    {
                        logging.SetMinimumLevel(LogLevel.Information);
                    })
                    .Build();
            });

            builder.Services.AddScoped<ChessHubService>();
            builder.Services.AddScoped<ChessControllerService>();

            await builder.Build().RunAsync();
        }
    }
}
