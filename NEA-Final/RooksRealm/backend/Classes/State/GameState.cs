namespace backend.Classes.State
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="GameState" />
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Gets or sets the board
        /// </summary>
        public List<List<string>> board { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether whiteToMove
        /// </summary>
        public bool whiteToMove { get; set; }

        /// <summary>
        /// Gets or sets the moveLog
        /// </summary>
        public List<Move> moveLog { get; set; }

        /// <summary>
        /// Gets or sets the whiteKingLocation
        /// </summary>
        public List<int> whiteKingLocation { get; set; }

        /// <summary>
        /// Gets or sets the blackKingLocation
        /// </summary>
        public List<int> blackKingLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether inCheck
        /// </summary>
        public bool inCheck { get; set; }

        /// <summary>
        /// Gets or sets the pins
        /// </summary>
        public List<List<int>> pins { get; set; }

        /// <summary>
        /// Gets or sets the checks
        /// </summary>
        public List<List<int>> checks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether checkMate
        /// </summary>
        public bool checkMate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether staleMate
        /// </summary>
        public bool staleMate { get; set; }

        /// <summary>
        /// Gets or sets the enPassantPossible
        /// </summary>
        public List<int>? enPassantPossible { get; set; }

        /// <summary>
        /// Gets or sets the currentCastlingRight
        /// </summary>
        public CastleRights currentCastlingRight { get; set; }

        /// <summary>
        /// Gets or sets the castleRightsLog
        /// </summary>
        public List<CastleRights> castleRightsLog { get; set; }

        /// <summary>
        /// Gets or sets the whiteTime
        /// </summary>
        public int whiteTime { get; set; }

        /// <summary>
        /// Gets or sets the blackTime
        /// </summary>
        public int blackTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether whiteTimeRunning
        /// </summary>
        public bool whiteTimeRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether blackTimeRunning
        /// </summary>
        public bool blackTimeRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gameOver
        /// </summary>
        public bool gameOver { get; set; }

        /// <summary>
        /// Gets or sets the fiftyMoveCounter
        /// </summary>
        public int fiftyMoveCounter { get; set; }

        /// <summary>
        /// Gets or sets the threeFoldCounter
        /// </summary>
        public int threeFoldCounter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether drawAgreed
        /// </summary>
        public bool drawAgreed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether playerResigned
        /// </summary>
        public bool playerResigned { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isWhiteResignation
        /// </summary>
        public bool isWhiteResignation { get; set; }

        /// <summary>
        /// Gets or sets the drawOffers
        /// </summary>
        public List<string> drawOffers { get; set; }

        /// <summary>
        /// Gets or sets the pauseRequests
        /// </summary>
        public List<string> pauseRequests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pauseAgreed
        /// </summary>
        public bool pauseAgreed { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        public GameState()
        {
            board = new List<List<string>>
            {
                new List<string> { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" },
                new List<string> { "bP", "bP", "bP", "bP", "bP", "bP", "bP", "bP" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "wP", "wP", "wP", "wP", "wP", "wP", "wP", "wP" },
                new List<string> { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }
            };
            whiteToMove = true;
            moveLog = new List<Move>();
            whiteKingLocation = new List<int> { 7, 4 };
            blackKingLocation = new List<int> { 0, 4 };
            inCheck = false;
            pins = new List<List<int>>();
            checks = new List<List<int>>();
            checkMate = false;
            staleMate = false;
            enPassantPossible = null;
            currentCastlingRight = new CastleRights(true, true, true, true);
            castleRightsLog = new List<CastleRights>();
            whiteTime = 600;
            blackTime = 600;
            whiteTimeRunning = false;
            blackTimeRunning = false;
            gameOver = false;
            fiftyMoveCounter = 0;
            drawAgreed = false;
            playerResigned = false;
            isWhiteResignation = false;
            drawOffers = new List<string>();
            pauseRequests = new List<string>();
            pauseAgreed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        /// <param name="original">The original<see cref="GameState"/></param>
        public GameState(GameState original)
        {
            this.board = original.board.Select(row => row.ToList()).ToList();
            this.whiteToMove = original.whiteToMove;
            this.moveLog = original.moveLog.Select(m => new Move(m)).ToList();
            this.whiteKingLocation = new List<int>(original.whiteKingLocation);
            this.blackKingLocation = new List<int>(original.blackKingLocation);
            this.inCheck = original.inCheck;
            this.pins = original.pins.Select(pin => new List<int>(pin)).ToList();
            this.checks = original.checks.Select(check => new List<int>(check)).ToList();
            this.checkMate = original.checkMate;
            this.staleMate = original.staleMate;
            this.enPassantPossible = original.enPassantPossible == null ? null : new List<int>(original.enPassantPossible);
            this.currentCastlingRight = new CastleRights(original.currentCastlingRight);
            this.castleRightsLog = original.castleRightsLog.Select(cr => new CastleRights(cr)).ToList();
            this.whiteTime = original.whiteTime;
            this.blackTime = original.blackTime;
            this.whiteTimeRunning = original.whiteTimeRunning;
            this.blackTimeRunning = original.blackTimeRunning;
            this.gameOver = original.gameOver;
            this.fiftyMoveCounter = original.fiftyMoveCounter;
            this.drawAgreed = original.drawAgreed;
            this.playerResigned = original.playerResigned;
            this.isWhiteResignation = original.isWhiteResignation;
            this.drawOffers = original.drawOffers;
            this.pauseRequests = original.pauseRequests;
            this.pauseAgreed = original.pauseAgreed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameState"/> class.
        /// </summary>
        /// <param name="board">The board<see cref="List{List{string}}"/></param>
        /// <param name="whiteToMove">The whiteToMove<see cref="bool"/></param>
        /// <param name="moveLog">The moveLog<see cref="List{Move}"/></param>
        /// <param name="whiteKingLocation">The whiteKingLocation<see cref="List{int}"/></param>
        /// <param name="blackKingLocation">The blackKingLocation<see cref="List{int}"/></param>
        /// <param name="inCheck">The inCheck<see cref="bool"/></param>
        /// <param name="pins">The pins<see cref="List{List{int}}"/></param>
        /// <param name="checks">The checks<see cref="List{List{int}}"/></param>
        /// <param name="checkMate">The checkMate<see cref="bool"/></param>
        /// <param name="staleMate">The staleMate<see cref="bool"/></param>
        /// <param name="enPassantPossible">The enPassantPossible<see cref="List{int}?"/></param>
        /// <param name="currentCastlingRight">The currentCastlingRight<see cref="CastleRights"/></param>
        /// <param name="castleRightsLog">The castleRightsLog<see cref="List{CastleRights}"/></param>
        /// <param name="whiteTime">The whiteTime<see cref="int"/></param>
        /// <param name="blackTime">The blackTime<see cref="int"/></param>
        /// <param name="whiteTimeRunning">The whiteTimeRunning<see cref="bool"/></param>
        /// <param name="blackTimeRunning">The blackTimeRunning<see cref="bool"/></param>
        /// <param name="gameOver">The gameOver<see cref="bool"/></param>
        /// <param name="fiftyMoveCounter">The fiftyMoveCounter<see cref="int"/></param>
        /// <param name="drawAgreed">The drawAgreed<see cref="bool"/></param>
        /// <param name="playerResigned">The playerResigned<see cref="bool"/></param>
        /// <param name="isWhiteResignation">The isWhiteResignation<see cref="bool"/></param>
        /// <param name="drawOffers">The drawOffers<see cref="List{string}"/></param>
        /// <param name="pauseRequests">The pauseRequests<see cref="List{string}"/></param>
        /// <param name="pauseAgreed">The pauseAgreed<see cref="bool"/></param>
        [JsonConstructor]
        public GameState(List<List<string>> board, bool whiteToMove, List<Move> moveLog,
            List<int> whiteKingLocation, List<int> blackKingLocation, bool inCheck,
            List<List<int>> pins, List<List<int>> checks, bool checkMate, bool staleMate,
            List<int>? enPassantPossible, CastleRights currentCastlingRight,
            List<CastleRights> castleRightsLog, int whiteTime, int blackTime,
            bool whiteTimeRunning, bool blackTimeRunning, bool gameOver,
            int fiftyMoveCounter, bool drawAgreed, bool playerResigned,
            bool isWhiteResignation, List<string> drawOffers, List<string> pauseRequests,
            bool pauseAgreed)
        {
            this.board = board;
            this.whiteToMove = whiteToMove;
            this.moveLog = moveLog;
            this.whiteKingLocation = whiteKingLocation;
            this.blackKingLocation = blackKingLocation;
            this.inCheck = inCheck;
            this.pins = pins;
            this.checks = checks;
            this.checkMate = checkMate;
            this.staleMate = staleMate;
            this.enPassantPossible = enPassantPossible;
            this.currentCastlingRight = currentCastlingRight;
            this.castleRightsLog = castleRightsLog;
            this.whiteTime = whiteTime;
            this.blackTime = blackTime;
            this.blackTimeRunning = blackTimeRunning;
            this.whiteTimeRunning = whiteTimeRunning;
            this.gameOver = gameOver;
            this.fiftyMoveCounter = fiftyMoveCounter;
            this.drawAgreed = drawAgreed;
            this.playerResigned = playerResigned;
            this.isWhiteResignation = isWhiteResignation;
            this.drawOffers = drawOffers;
            this.pauseRequests = pauseRequests;
            this.pauseAgreed = pauseAgreed;
        }
    }
}
