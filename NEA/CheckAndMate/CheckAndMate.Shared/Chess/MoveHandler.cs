using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public static class MoveHandler
    {
        public static bool MovesEqual(Move move1, Move move2)
        {
            return (move1.moveID == move2.moveID);
        }

        public static string GetChessNotation(Move move)
        {
            return GetRankFile(move.startRow, move.startCol) + GetRankFile(move.endRow, move.endCol);
        }

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
