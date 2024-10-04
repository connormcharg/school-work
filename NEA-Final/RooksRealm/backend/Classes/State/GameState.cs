using Newtonsoft.Json;

namespace backend.Classes.State
{
    public class GameState
    {
        public List<List<string>> board { get; set; }
        public bool whiteToMove { get; set; }
        public List<Move> moveLog { get; set; }
        public List<int> whiteKingLocation { get; set; }
        public List<int> blackKingLocation { get; set; }
        public bool inCheck { get; set; }
        public List<List<int>> pins { get; set; }
        public List<List<int>> checks { get; set; }
        public bool checkMate { get; set; }
        public bool staleMate { get; set; }
        public List<int>? enPassantPossible { get; set; }
        public CastleRights currentCastlingRight { get; set; }
        public List<CastleRights> castleRightsLog { get; set; }
        public int whiteTime { get; set; }
        public int blackTime { get; set; }
        public bool whiteTimeRunning { get; set; }
        public bool blackTimeRunning { get; set; }

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
        }

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
        }

        [JsonConstructor]
        public GameState(List<List<string>> board, bool whiteToMove, List<Move> moveLog,
            List<int> whiteKingLocation, List<int> blackKingLocation, bool inCheck,
            List<List<int>> pins, List<List<int>> checks, bool checkMate, bool staleMate,
            List<int>? enPassantPossible, CastleRights currentCastlingRight,
            List<CastleRights> castleRightsLog, int whiteTime, int blackTime,
            bool whiteTimeRunning, bool blackTimeRunning)
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
        }
    }
}
