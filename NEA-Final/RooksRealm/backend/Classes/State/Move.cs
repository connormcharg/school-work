namespace backend.Classes.State
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="Move" />
    /// </summary>
    public class Move
    {
        /// <summary>
        /// Gets or sets the startRow
        /// </summary>
        public int startRow { get; set; }

        /// <summary>
        /// Gets or sets the startCol
        /// </summary>
        public int startCol { get; set; }

        /// <summary>
        /// Gets or sets the endRow
        /// </summary>
        public int endRow { get; set; }

        /// <summary>
        /// Gets or sets the endCol
        /// </summary>
        public int endCol { get; set; }

        /// <summary>
        /// Gets or sets the pieceMoved
        /// </summary>
        public string pieceMoved { get; set; }

        /// <summary>
        /// Gets or sets the pieceCaptured
        /// </summary>
        public string pieceCaptured { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enPassant
        /// </summary>
        public bool enPassant { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pawnPromotion
        /// </summary>
        public bool pawnPromotion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether isCastleMove
        /// </summary>
        public bool isCastleMove { get; set; }

        /// <summary>
        /// Gets or sets the moveID
        /// </summary>
        public int moveID { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// </summary>
        /// <param name="startRow">The startRow<see cref="int"/></param>
        /// <param name="startCol">The startCol<see cref="int"/></param>
        /// <param name="endRow">The endRow<see cref="int"/></param>
        /// <param name="endCol">The endCol<see cref="int"/></param>
        /// <param name="board">The board<see cref="List{List{string}}"/></param>
        /// <param name="enPassant">The enPassant<see cref="bool"/></param>
        /// <param name="pawnPromotion">The pawnPromotion<see cref="bool"/></param>
        /// <param name="isCastleMove">The isCastleMove<see cref="bool"/></param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// </summary>
        /// <param name="original">The original<see cref="Move"/></param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class.
        /// </summary>
        /// <param name="startRow">The startRow<see cref="int"/></param>
        /// <param name="startCol">The startCol<see cref="int"/></param>
        /// <param name="endRow">The endRow<see cref="int"/></param>
        /// <param name="endCol">The endCol<see cref="int"/></param>
        /// <param name="pieceMoved">The pieceMoved<see cref="string"/></param>
        /// <param name="pieceCaptured">The pieceCaptured<see cref="string"/></param>
        /// <param name="enPassant">The enPassant<see cref="bool"/></param>
        /// <param name="pawnPromotion">The pawnPromotion<see cref="bool"/></param>
        /// <param name="isCastleMove">The isCastleMove<see cref="bool"/></param>
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
