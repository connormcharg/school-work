namespace backend.Classes.State
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Player" />
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the connectionId
        /// </summary>
        public string connectionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isWhite
        /// </summary>
        public bool isWhite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isHost
        /// </summary>
        public bool isHost { get; set; }

        /// <summary>
        /// Gets or sets the nickName
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isConnected
        /// </summary>
        public bool isConnected { get; set; }

        /// <summary>
        /// Gets or sets the rating
        /// </summary>
        public int rating { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="connectionId">The connectionId<see cref="string"/></param>
        public Player(string connectionId)
        {
            this.connectionId = connectionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="original">The original<see cref="Player"/></param>
        public Player(Player original)
        {
            this.connectionId = original.connectionId;
            this.isWhite = original.isWhite;
            this.isHost = original.isHost;
            this.nickName = original.nickName;
            this.isConnected = original.isConnected;
            this.rating = original.rating;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="connectionId">The connectionId<see cref="string"/></param>
        /// <param name="isWhite">The isWhite<see cref="bool"/></param>
        /// <param name="isHost">The isHost<see cref="bool"/></param>
        /// <param name="nickName">The nickName<see cref="string"/></param>
        /// <param name="isConnected">The isConnected<see cref="bool"/></param>
        /// <param name="rating">The rating<see cref="int"/></param>
        [JsonConstructor]
        public Player(string connectionId, bool isWhite, bool isHost, string nickName, bool isConnected, int rating)
        {
            this.connectionId = connectionId;
            this.isWhite = isWhite;
            this.isHost = isHost;
            this.nickName = nickName;
            this.isConnected = isConnected;
            this.rating = rating;
        }
    }
}
