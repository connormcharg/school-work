using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Position
    {
        public string[][] board = new string[][]
        {
            new string[] { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" },
            new string[] { "bP", "bP", "bP", "bP", "bP", "bP", "bP", "bP" },
            new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
            new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
            new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
            new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
            new string[] { "wP", "wP", "wP", "wP", "wP", "wP", "wP", "wP" },
            new string[] { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }
        };
        public bool whiteToMove = true;
        public int[] whiteKingLocation = new int[] { 7, 4 };
        public int[] blackKingLocation = new int[] { 0, 4 };
        public CastleRights currentCastlingRight = new CastleRights(true, true, true, true);
        public List<CastleRights> castleRightsLog;
        public int[]? isEnPassantPossible = null;

        public Position()
        {
            this.castleRightsLog = new List<CastleRights> { new CastleRights(
                currentCastlingRight.wks,
                currentCastlingRight.bks,
                currentCastlingRight.wqs,
                currentCastlingRight.bqs) };
        }

        public override string ToString()
        {
            return Util.GetFenString(this);
        }
    }
}
