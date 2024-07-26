using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public class Move
    {
        private Dictionary<int, string> rowsToRanks = new() {
            {0, "8"}, {1, "7"}, {2, "6"}, {3, "5"}, {4, "4"}, {5, "3"}, {6, "2"}, {7, "1"}
        };
        private Dictionary<int, string> colsToFiles = new() {
            {0, "a"}, {1, "b"}, {2, "c"}, {3, "d"}, {4, "e"}, {5, "f"}, {6, "g"}, {7, "h"}
        };

        public (int row, int col) start;
        public (int row, int col) end;
        public IPiece pieceMoved;
        public IPiece pieceCaptured;
        public bool isPawnPromotion;
        public bool isEnPassant;
        public bool isCastle;
        public bool isCapture;
        public int moveID;

        public Move((int, int) start, (int, int) end, Board board, bool isEnpassant, bool isCastle)
        {
            this.start = start;
            this.end = end;
            this.pieceMoved = board[start];
            this.pieceCaptured = board[end];

            this.isPawnPromotion = (this.pieceMoved.ToString() == "wP" && this.end.row == 0) || 
                (this.pieceMoved.ToString() == "bP" && this.end.row == 7);
            this.isEnPassant = isEnpassant;
            if (this.isEnPassant)
            {
                this.pieceCaptured = this.pieceMoved.ToString() == "bP" ? new Pawn(true) : new Pawn(false);
            }           
            this.isCastle = isCastle;
            this.isCapture = this.pieceCaptured.ToString() != "--";

            this.moveID = this.start.row * 1000 + this.start.col * 100 + this.end.row * 10 + this.end.col;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Move)
            {
                return this.moveID == ((Move)obj).moveID;
            }
            return false;
        }
    }
}
