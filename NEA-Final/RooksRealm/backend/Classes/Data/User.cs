namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="User" />
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string? username { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string? email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether emailConfirmed
        /// </summary>
        public bool emailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the storedHashValue
        /// </summary>
        public string? storedHashValue { get; set; }

        /// <summary>
        /// Gets or sets the boardTheme
        /// </summary>
        public string? boardTheme { get; set; }

        /// <summary>
        /// Gets or sets the rating
        /// </summary>
        public int rating { get; set; }

        /// <summary>
        /// Gets or sets the role
        /// </summary>
        public string? role { get; set; }
    }
}
