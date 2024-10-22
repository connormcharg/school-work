using CheckAndMate.Shared.Chess;
using System.Net.Http.Json;

namespace CheckAndMate.Client.Services
{
    public class ChessControllerService
    {
        private readonly HttpClient _httpClient;

        public ChessControllerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> StartGameAsync(GameSettings gameSettings)
        {
            var response = await _httpClient.PostAsJsonAsync("api/chess/start", gameSettings);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<StartGameResponse>();
                return result?.id;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                // Bad Request: invalid input or server error.
                return null;
            }
            else
            {
                // Other Error.
                return null;
            }
        }

        private class StartGameResponse
        {
            public string? id { get; set; }
        }
    }
}
