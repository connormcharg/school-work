using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckAndMate.Shared.Messages;

namespace CheckAndMate.Shared.Chess
{
    public class Game
    {
        public GameState gameState { get; set; }
        public List<Move> currentValidMoves { get; set; }
        public string id { get; set; }
        public List<Player> players { get; set; }
        public List<string> watcherConnections { get; set; }
        public List<IMessage> messages { get; set; }
        public GameSettings settings { get; set; }

        public Game(GameSettings Settings)
        {
            settings = Settings;
            gameState = new GameState();
            currentValidMoves = GameHandler.FindValidMoves(this);
            players = new List<Player>();
            watcherConnections = new List<string>();
            messages = new List<IMessage>();
        }
    }
}
