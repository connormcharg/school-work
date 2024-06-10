using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Util
    {
        public static Dictionary<int, string> rowsToRanks = new Dictionary<int, string> {
            { 7, "1" }, { 6, "2" }, { 5, "3" }, { 4, "4" },
            { 3, "5" }, { 2, "6" }, { 1, "7" }, { 0, "8" }
        };
        public static Dictionary<int, string> colsToFiles = new Dictionary<int, string> {
            { 0, "a" }, { 1, "b" }, { 2, "c" }, { 3, "d" },
            { 4, "e" }, { 5, "f" }, { 6, "g" }, { 7, "h" }
        };

        public static string GetChessNotation(int[] _start, int[] _end)
        {
            return GetRankFile(_start) + GetRankFile(_end);
        }

        public static string GetRankFile(int[] _location)
        {
            return colsToFiles[_location[0]] + rowsToRanks[_location[1]];
        }

        public static string GetFenString(Position _position)
        {
            string fen = "";
            int index = 0;

            foreach (string[] row in _position.board)
            {
                int count = 0;
                foreach (string square in row)
                {
                    if (square == "--")
                    {
                        count++;
                    }
                    else
                    {
                        fen += count != 0 ? count.ToString() : "";
                        count = 0;
                        fen += square[0].ToString() == "w" ?
                        square[1].ToString().ToUpper() :
                        square[1].ToString().ToLower();
                    }
                }
                fen += count != 0 ? count.ToString() : "";
                fen += (index != 7) ? "/" : "";
                index++;
            }
            fen += " ";

            fen += _position.whiteToMove ? "w" : "b";
            fen += " ";

            fen += _position.currentCastlingRight.wks ? "K" : "-";
            fen += _position.currentCastlingRight.wqs ? "Q" : "-";
            fen += _position.currentCastlingRight.bks ? "k" : "-";
            fen += _position.currentCastlingRight.bqs ? "q" : "-";
            fen += " ";

            fen += (_position.isEnPassantPossible is not null) ?
                colsToFiles[_position.isEnPassantPossible[1]] + rowsToRanks[_position.isEnPassantPossible[0]] :
                "-";

            return fen;
        }
    }
}
