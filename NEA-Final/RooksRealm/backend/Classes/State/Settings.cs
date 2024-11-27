namespace backend.Classes.State
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Settings" />
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets a value indicating whether isSinglePlayer
        /// </summary>
        public bool isSinglePlayer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isStartingWhite
        /// </summary>
        public bool isStartingWhite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isTimed
        /// </summary>
        public bool isTimed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isPrivate
        /// </summary>
        public bool isPrivate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isRated
        /// </summary>
        public bool isRated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isWatchable
        /// </summary>
        public bool isWatchable { get; set; }

        /// <summary>
        /// Gets or sets the gameTitle
        /// </summary>
        public string gameTitle { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.isSinglePlayer = false;
            this.isStartingWhite = true;
            this.isTimed = false;
            this.isPrivate = false;
            this.isRated = false;
            this.isWatchable = true;
            this.gameTitle = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="original">The original<see cref="Settings"/></param>
        public Settings(Settings original)
        {
            this.isSinglePlayer = original.isSinglePlayer;
            this.isStartingWhite = original.isStartingWhite;
            this.isTimed = original.isTimed;
            this.isPrivate = original.isPrivate;
            this.isRated = original.isRated;
            this.isWatchable = original.isWatchable;
            this.gameTitle = original.gameTitle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="isSinglePlayer">The isSinglePlayer<see cref="bool"/></param>
        /// <param name="isStartingWhite">The isStartingWhite<see cref="bool"/></param>
        /// <param name="isTimed">The isTimed<see cref="bool"/></param>
        /// <param name="isPrivate">The isPrivate<see cref="bool"/></param>
        /// <param name="isRated">The isRated<see cref="bool"/></param>
        /// <param name="isWatchable">The isWatchable<see cref="bool"/></param>
        /// <param name="gameTitle">The gameTitle<see cref="string"/></param>
        [JsonConstructor]
        public Settings(bool isSinglePlayer, bool isStartingWhite, bool isTimed, bool isPrivate,
            bool isRated, bool isWatchable, string gameTitle)
        {
            this.isSinglePlayer = isSinglePlayer;
            this.isStartingWhite = isStartingWhite;
            this.isTimed = isTimed;
            this.isPrivate = isPrivate;
            this.isRated = isRated;
            this.isWatchable = isWatchable;
            this.gameTitle = gameTitle;
        }
    }
}
