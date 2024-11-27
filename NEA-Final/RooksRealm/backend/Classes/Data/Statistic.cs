namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="Statistic" />
    /// </summary>
    public class Statistic
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the avgMoveTime
        /// </summary>
        public double avgMoveTime { get; set; }

        /// <summary>
        /// Gets or sets the numberOfMoves
        /// </summary>
        public int numberOfMoves { get; set; }

        /// <summary>
        /// Gets or sets the userId
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// Gets or sets the gameId
        /// </summary>
        public int gameId { get; set; }

        /// <summary>
        /// Gets or sets the outcome
        /// </summary>
        public string? outcome { get; set; }

        /// <summary>
        /// Gets or sets the datetime
        /// </summary>
        public DateTime datetime { get; set; }
    }
}
