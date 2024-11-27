namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="Message" />
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string? content { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string? username { get; set; }

        /// <summary>
        /// Gets or sets the datetime
        /// </summary>
        public DateTime datetime { get; set; }
    }
}
