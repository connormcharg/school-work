using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public class GameSettings
    {
        public bool isSinglePlayer { get; set; }
        public bool isStartingWhite { get; set; }
        public bool isTimed { get; set; }
        public bool isPrivate { get; set; }
        public bool isRated { get; set; }
        public bool isWatchable { get; set; }
        public string gameTitle { get; set; }

        [JsonConstructor]
        public GameSettings(bool isSinglePlayer, bool isStartingWhite, bool isTimed, bool isPrivate,
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

        public GameSettings()
        {
            isSinglePlayer = false;
            isStartingWhite = true;
            isTimed = false;
            isPrivate = false;
            isRated = false;
            isWatchable = true;
            gameTitle = string.Empty;
        }

        public GameSettings(GameSettings original)
        {
            isSinglePlayer = original.isSinglePlayer;
            isStartingWhite = original.isStartingWhite;
            isTimed = original.isTimed;
            isPrivate = original.isPrivate;
            isRated = original.isRated;
            isWatchable = original.isWatchable;
            gameTitle = original.gameTitle;
        }
    }
}
