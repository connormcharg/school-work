using Newtonsoft.Json;
using backend.Classes.Handlers;

namespace backend.Classes.State
{
    public class Game
    {
        public GameState state { get; set; }
        public List<Move> currentValidMoves { get; set; }
        public string id { get; set; }
        public List<Player> players { get; set; }
        public List<string> watchers { get; set; }
        /*public List<IMessage> messages { get; set; }*/
        public Settings settings { get; set; }
        public int suggestedMoveId { get; set; }

        public Game(Settings settings)
        { 
            this.state = new GameState();
            this.currentValidMoves = GameHandler.FindValidMoves(this);
            this.id = string.Empty;
            this.players = new List<Player>();
            this.watchers = new List<string>();
            /*this.messages = new List<IMessage>();*/
            this.settings = settings;
            this.suggestedMoveId = 0;
        }

        public Game(Game original)
        {
            this.state = new GameState(original.state);
            this.currentValidMoves = original.currentValidMoves.Select(m => new Move(m)).ToList();
            this.id = original.id;
            this.players = original.players.Select(p => new Player(p)).ToList();
            this.watchers = new List<string>(original.watchers);
            /*this.messages = original.messages.Select(m => m.DeepCopy()).ToList();*/
            this.settings = new Settings(original.settings);
            this.suggestedMoveId = original.suggestedMoveId;
        }

        [JsonConstructor]
        public Game(GameState state, List<Move> currentValidMoves, string id, List<Player> players,
            List<string> watchers, /*List<IMessage> messages,*/ Settings settings, int suggestedMoveId)
        {
            this.state = state;
            this.currentValidMoves = currentValidMoves;
            this.id = id;
            this.players = players;
            this.watchers = watchers;
            /*this.messages = messages;*/
            this.settings = settings;
            this.suggestedMoveId = suggestedMoveId;
        }
    }
}
