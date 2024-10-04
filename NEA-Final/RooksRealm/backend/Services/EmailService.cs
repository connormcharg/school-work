using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text;
using System.Net.Http.Headers;

namespace backend.Services
{
    public class EmailService : IEmailSender
    {
        private readonly string? apiKey;
        private readonly string domain;
        private readonly HttpClient httpClient;

        public EmailService(HttpClient httpClient)
        {
            this.apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY");
            this.domain = "mai.mcharg.uk";
            this.httpClient = httpClient;
        }

        public async Task SendEmailAsync(string to, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("Null MailGunApiKey");
            }

            var requestUrl = $"https://api.eu.mailgun.net/v3/{domain}/messages";

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", $"Rook's Realm <chess@{domain}>"),
                new KeyValuePair<string, string>("to", to),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", message)
            });

            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api: {apiKey}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await httpClient.PostAsync(requestUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email: {response.ReasonPhrase}");
            }
        }
    }
}
