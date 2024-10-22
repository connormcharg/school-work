using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckAndMate.Shared.Messages;
using Newtonsoft.Json;

namespace CheckAndMate.Shared.Chess
{
    public class Game
    {
        public GameState gameState { get; set; }
        public List<Move> currentValidMoves { get; set; }
        public string id { get; set; }
        public List<Player> players { get; set; }
        public List<string> watcherConnections { get; set; }
        /*public List<IMessage> messages { get; set; }*/
        public GameSettings settings { get; set; }

        public Game(GameSettings Settings)
        {
            settings = Settings;
            gameState = new GameState();
            currentValidMoves = GameHandler.FindValidMoves(this);
            players = new List<Player>();
            watcherConnections = new List<string>();
            id = "";
            /*messages = new List<IMessage>();*/
        }

        [JsonConstructor]
        public Game(GameState gameState, List<Move> currentValidMoves, string id, List<Player> players,
            List<string> watcherConnections, GameSettings settings)
        {
            this.gameState = gameState;
            this.currentValidMoves = currentValidMoves;
            this.id = id;
            this.players = players;
            this.watcherConnections = watcherConnections;
            this.settings = settings;
        }

        public Game(Game original)
        {
            gameState = new GameState(original.gameState);
            currentValidMoves = original.currentValidMoves.Select(m => new Move(m)).ToList();
            id = original.id;
            players = original.players.Select(p => new Player(p)).ToList();
            watcherConnections = new List<string>(original.watcherConnections);
            /*messages = original.messages.Select(m => m.DeepCopy()).ToList();*/
            settings = new GameSettings(original.settings);
        }
    }
}
