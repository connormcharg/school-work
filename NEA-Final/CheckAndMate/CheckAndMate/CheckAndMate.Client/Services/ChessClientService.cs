using CheckAndMate.Shared.Chess;
using Microsoft.AspNetCore.SignalR.Client;

namespace CheckAndMate.Client.Services
{
    public class ChessClientService
    {
        private readonly HubConnection _hubConnection;
        public bool isConnected => _hubConnection?.State == HubConnectionState.Connected;

        public ChessClientService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public async Task StartAsync()
        {
            _hubConnection.On<Game>("ReceiveGame", game =>
            {
                OnGameReceived?.Invoke(game);
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMoveAsync(string gameId, Move move)
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

        public event Action<Game>? OnGameReceived;
    }
}
