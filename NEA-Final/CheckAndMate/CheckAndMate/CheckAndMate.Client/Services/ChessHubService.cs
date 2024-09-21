using CheckAndMate.Shared.Chess;
using Microsoft.AspNetCore.SignalR.Client;

namespace CheckAndMate.Client.Services
{
    public class ChessHubService
    {
        private readonly HubConnection _hubConnection;
        public event Action<string> OnGameReceived;
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public ChessHubService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public string GetConnectionId()
        {
            if (IsConnected && _hubConnection != null && _hubConnection.ConnectionId != null)
            {
                return _hubConnection.ConnectionId;
            }
            return "";
        }

        public async Task StartAsync()
        {
            _hubConnection.On<string>("ReceiveGame", (game) =>
            {
                OnGameReceived.Invoke(game);
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMoveAsync(string moveJson)
        {
            await _hubConnection.SendAsync("SendMove", moveJson);
        }

        public async Task JoinGameAsPlayerAsync(string gameId)
        {
            await _hubConnection.SendAsync("JoinGameAsPlayer", gameId);
        }

        public async Task LeaveGameAsPlayerAsync()
        {
            await _hubConnection.SendAsync("LeaveGameAsPlayer");
        }

        public async Task JoinGameAsWatcherAsync(string gameId)
        {
            await _hubConnection.SendAsync("JoinGameAsWatcher", gameId);
        }

        public async Task LeaveGameAsWatcherAsync()
        {
            await _hubConnection.SendAsync("LeaveGameAsWatcher");
        }
    }
}
