using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckAndMate.Shared.Connections;
using CheckAndMate.Shared.Messages;

namespace CheckAndMate.Shared.Chess
{
    public class Game
    {
        public GameState gameState { get; set; }
        public string id { get; set; }
        public List<ConnectionInfo> connections { get; set; }
        public List<IMessage> messages { get; set; }
        
        public GameSettings settings { get; set; }

        public Game(GameSettings Settings)
        {
            settings = Settings;
            gameState = new GameState();
        }
    }
}
