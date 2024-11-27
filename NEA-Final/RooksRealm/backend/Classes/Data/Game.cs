namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="Game" />
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the playerOneId
        /// </summary>
        public int playerOneId { get; set; }

        /// <summary>
        /// Gets or sets the playerTwoId
        /// </summary>
        public int playerTwoId { get; set; }

        /// <summary>
        /// Gets or sets the gameData
        /// </summary>
        public string? gameData { get; set; }
    }
}
