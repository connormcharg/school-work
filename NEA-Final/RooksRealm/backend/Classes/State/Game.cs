namespace backend.Classes.State
{
    using backend.Classes.Handlers;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Game" />
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public GameState state { get; set; }

        /// <summary>
        /// Gets or sets the currentValidMoves
        /// </summary>
        public List<Move> currentValidMoves { get; set; }

        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the players
        /// </summary>
        public List<Player> players { get; set; }

        /// <summary>
        /// Gets or sets the watchers
        /// </summary>
        public List<string> watchers { get; set; }

        /*public List<IMessage> messages { get; set; }*/

        /// <summary>
        /// Gets or sets the settings
        /// </summary>
        public Settings settings { get; set; }

        /// <summary>
        /// Gets or sets the suggestedMoveId
        /// </summary>
        public int suggestedMoveId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="settings">The settings<see cref="Settings"/></param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="original">The original<see cref="Game"/></param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="state">The state<see cref="GameState"/></param>
        /// <param name="currentValidMoves">The currentValidMoves<see cref="List{Move}"/></param>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="players">The players<see cref="List{Player}"/></param>
        /// <param name="watchers">The watchers<see cref="List{string}"/></param>
        /// <param name="settings">The settings<see cref="Settings"/></param>
        /// <param name="suggestedMoveId">The suggestedMoveId<see cref="int"/></param>
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
