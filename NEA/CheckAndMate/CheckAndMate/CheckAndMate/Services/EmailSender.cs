using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http.Headers;
using System.Text;

namespace CheckAndMate.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string? apiKey;
        private readonly string domain;
        private readonly HttpClient httpClient;

        public EmailSender(HttpClient HttpClient)
        {
            httpClient = HttpClient;
            domain = "mail.mcharg.uk";
            apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY");
        }

        public async Task SendEmailAsync(string recipient, string subject, string text)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("Null MailGunApiKey");
            }

            var requestUrl = $"https://api.eu.mailgun.net/v3/{domain}/messages";

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", $"CheckAndMate <chess@{domain}>"),
                new KeyValuePair<string, string>("to", recipient),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("text", text)
            });

            var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await httpClient.PostAsync(requestUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email: {response.ReasonPhrase}");
            }
        }
    }
}
