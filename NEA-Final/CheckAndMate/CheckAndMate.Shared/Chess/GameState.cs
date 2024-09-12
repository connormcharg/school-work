using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public class GameState
    {
        public List<List<string>> board = new List<List<string>>
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
        public bool whiteToMove = true;
        public List<Move> moveLog = new List<Move>();
        public List<int> whiteKingLocation = new List<int> { 7, 4 };
        public List<int> blackKingLocation = new List<int> { 0, 4 };
        public bool inCheck = false;
        public List<List<int>> pins = new List<List<int>>();
        public List<List<int>> checks = new List<List<int>>();
        public bool checkMate = false;
        public bool staleMate = false;
        public List<int>? enPassantPossible = null;
        public CastleRights currentCastlingRight = new CastleRights(true, true, true, true);
        public List<CastleRights> castleRightsLog;
        public int whiteTime = 600;
        public int blackTime = 600;
        public bool whiteTimeRunning = false;
        public bool blackTimeRunning = false;
    }
}
