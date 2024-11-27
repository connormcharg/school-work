namespace backend.Classes.Handlers
{
    using backend.Classes.State;

    /// <summary>
    /// Defines the <see cref="MoveHandler" />
    /// </summary>
    public static class MoveHandler
    {
        /// <summary>
        /// The MovesEqual
        /// </summary>
        /// <param name="move1">The move1<see cref="Move"/></param>
        /// <param name="move2">The move2<see cref="Move"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool MovesEqual(Move move1, Move move2)
        {
            return (move1.moveID == move2.moveID);
        }

        /// <summary>
        /// The GetChessNotation
        /// </summary>
        /// <param name="move">The move<see cref="Move"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetChessNotation(Move move)
        {
            return GetRankFile(move.startRow, move.startCol) + GetRankFile(move.endRow, move.endCol);
        }

        /// <summary>
        /// The GetRankFile
        /// </summary>
        /// <param name="row">The row<see cref="int"/></param>
        /// <param name="col">The col<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        private static string GetRankFile(int row, int col)
        {
            Dictionary<int, string> rowsToRanks = new Dictionary<int, string>
            {
                { 7, "1" }, { 6, "2" }, { 5, "3" }, { 4, "4" },
                { 3, "5" }, { 2, "6" }, { 1, "7" }, { 0, "8" }
            };
            Dictionary<int, string> colsToFiles = new Dictionary<int, string>
            {
                { 0, "a" }, { 1, "b" }, { 2, "c" }, { 3, "d" },
                { 4, "e" }, { 5, "f" }, { 6, "g" }, { 7, "h" }
            };

            return colsToFiles[col] + rowsToRanks[row];
        }
    }
}
