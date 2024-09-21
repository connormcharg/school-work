using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public class Move
    {
        public int startRow { get; set; }
        public int startCol { get; set; }
        public int endRow { get; set; }
        public int endCol { get; set; }
        public string pieceMoved { get; set; }
        public string pieceCaptured { get; set; }
        public bool enPassant { get; set; }
        public bool pawnPromotion { get; set; }
        public bool isCastleMove { get; set; }
        public int moveID { get; set; }

        [JsonConstructor]
        public Move(int StartRow, int StartCol, int EndRow, int EndCol, string pieceMoved, string pieceCaptured,
            bool enPassant = false, bool pawnPromotion = false, bool isCastleMove = false)
        {
            this.startRow = StartRow;
            this.startCol = StartCol;
            this.endRow = EndRow;
            this.endCol = EndCol;
            this.pieceMoved = pieceMoved;
            this.pieceCaptured = pieceCaptured;
            this.enPassant = enPassant;
            this.pawnPromotion = pawnPromotion;
            this.isCastleMove = isCastleMove;
            if (this.enPassant)
            {
                this.pieceCaptured = this.pieceMoved == "wP" ? "bP" : "wP";
            }
            this.moveID = this.startRow * 1000 + this.startCol * 100 + this.endRow * 10 + this.endCol;
        }

        public Move(int StartRow, int StartCol, int EndRow, int EndCol, List<List<string>> board,
            bool enPassant = false, bool pawnPromotion = false, bool isCastleMove = false)
        {
            this.startRow = StartRow;
            this.startCol = StartCol;
            this.endRow = EndRow;
            this.endCol = EndCol;
            this.pieceMoved = board[startRow][StartCol];
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
