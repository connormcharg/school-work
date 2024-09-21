using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
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
