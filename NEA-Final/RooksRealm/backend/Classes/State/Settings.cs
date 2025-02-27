namespace backend.Classes.State
{
    using Newtonsoft.Json;

    public class Settings
    {
        public bool isSinglePlayer { get; set; }
        public bool isStartingWhite { get; set; }
        public bool isTimed { get; set; }
        public bool isPrivate { get; set; }
        public bool isRated { get; set; }
        public bool isWatchable { get; set; }
        public string gameTitle { get; set; }

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