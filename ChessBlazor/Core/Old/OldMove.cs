using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class OldMove
    {
        public Dictionary<int, string> rowsToRanks = new Dictionary<int, string> {
            { 7, "1" }, { 6, "2" }, { 5, "3" }, { 4, "4" },
            { 3, "5" }, { 2, "6" }, { 1, "7" }, { 0, "8" }
        };
        public Dictionary<int, string> colsToFiles = new Dictionary<int, string> {
            { 0, "a" }, { 1, "b" }, { 2, "c" }, { 3, "d" },
            { 4, "e" }, { 5, "f" }, { 6, "g" }, { 7, "h" }
        };
        public int startRow;
        public int startCol;
        public int endRow;
        public int endCol;
        public string pieceMoved;
        public string pieceCaptured;
        public bool enPassant;
        public bool pawnPromotion;
        public bool isCastleMove;
        public int moveID;

        public OldMove(List<int> start_square, List<int> end_square, List<List<string>> board,
            bool enPassant = false, bool pawnPromotion = false, bool isCastleMove = false)
        {
            this.startRow = start_square[0];
            this.startCol = start_square[1];
            this.endRow = end_square[0];
            this.endCol = end_square[1];
            this.pieceMoved = board[startRow][startCol];
            this.pieceCaptured = board[endRow][endCol];
            this.enPassant = enPassant;
            this.pawnPromotion = pawnPromotion;
            this.isCastleMove = isCastleMove;
            if (this.enPassant)
            {
                this.pieceCaptured = this.pieceMoved == "wP" ? "bP" : "wP";
            }
            this.moveID = this.startRow * 1000 + this.startCol * 100 + this.endRow * 10 + this.endCol;
        }

        public string GetChessNotation()
        {
            return getRankFile(this.startRow, this.startCol) + getRankFile(this.endRow, this.endCol);
        }

        public string getRankFile(int row, int col)
        {
            return this.colsToFiles[col] + this.rowsToRanks[row];
        }

        public override bool Equals(object? obj)
        {
            if (obj is OldMove)
            {
                OldMove move = (OldMove)obj;
                return this.moveID == move.moveID;
            }
            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
