using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http.Headers;
using System.Text;

namespace CheckAndMate.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string? _apiKey;
        private readonly string _domain;
        private readonly HttpClient _httpClient;

        public EmailSender(HttpClient httpClient)
        {
            _apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY");
            _domain = "mail.mcharg.uk";
            _httpClient = httpClient;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new Exception("Null MailGunApiKey");
            }

            var requestUrl = $"https://api.eu.mailgun.net/v3/{_domain}/messages";

            var requestContent = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("from", $"CheckAndMate <chess@{_domain}>"),
                new KeyValuePair<string, string>("to", toEmail),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", message)
            });

            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_apiKey}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await _httpClient.PostAsync(requestUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email: {response.ReasonPhrase}");
            }
        }
    }
}
