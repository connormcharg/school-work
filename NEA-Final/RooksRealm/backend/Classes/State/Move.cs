using Newtonsoft.Json;

namespace backend.Classes.State
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

        public Move(int startRow, int startCol, int endRow, int endCol, List<List<string>> board,
                bool enPassant = false, bool pawnPromotion = false, bool isCastleMove = false)
        {
            this.startRow = startRow;
            this.startCol = startCol;
            this.endRow = endRow;
            this.endCol = endCol;
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

        public Move(Move original)
        {
            startRow = original.startRow;
            startCol = original.startCol;
            endRow = original.endRow;
            endCol = original.endCol;
            pieceMoved = original.pieceMoved;
            pieceCaptured = original.pieceCaptured;
            enPassant = original.enPassant;
            pawnPromotion = original.pawnPromotion;
            isCastleMove = original.isCastleMove;
            moveID = original.moveID;
        }

        [JsonConstructor]
        public Move(int startRow, int startCol, int endRow, int endCol, string pieceMoved, string pieceCaptured,
            bool enPassant = false, bool pawnPromotion = false, bool isCastleMove = false)
        {
            this.startRow = startRow;
            this.startCol = startCol;
            this.endRow = endRow;
            this.endCol = endCol;
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
    }
}
