using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public class Move
    {
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

        public Move(List<int> start_square, List<int> end_square, List<List<string>> board,
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
    }
}
