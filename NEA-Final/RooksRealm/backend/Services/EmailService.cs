namespace backend.Services
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using System.Net.Http.Headers;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="EmailService" />
    /// </summary>
    public class EmailService : IEmailSender
    {
        /// <summary>
        /// Defines the apiKey
        /// </summary>
        private readonly string? apiKey;

        /// <summary>
        /// Defines the domain
        /// </summary>
        private readonly string domain;

        /// <summary>
        /// Defines the httpClient
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="httpClient">The httpClient<see cref="HttpClient"/></param>
        public EmailService(HttpClient httpClient)
        {
            this.apiKey = Environment.GetEnvironmentVariable("MAILGUN_API_KEY");
            this.domain = "mai.mcharg.uk";
            this.httpClient = httpClient;
        }

        /// <summary>
        /// The SendEmailAsync
        /// </summary>
        /// <param name="to">The to<see cref="string"/></param>
        /// <param name="subject">The subject<see cref="string"/></param>
        /// <param name="message">The message<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
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
