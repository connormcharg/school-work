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

        public GameSettings(bool IsSinglePlayer, bool IsStartingWhite, bool IsTimed, bool IsPrivate,
            bool IsRated, bool IsWatchable, string GameTitle)
        {
            isSinglePlayer = IsSinglePlayer;
            isStartingWhite = IsStartingWhite;
            isTimed = IsTimed;
            isPrivate = IsPrivate;
            isRated = IsRated;
            isWatchable = IsWatchable;
            gameTitle = GameTitle;
        }
    }
}
