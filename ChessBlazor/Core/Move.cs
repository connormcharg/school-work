using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Move
    {
        public int[] start;
        public int[] end;
        public string pieceMoved;
        public string pieceCaptured;
        public bool isEnPassant;
        public bool isPawnPromotion;
        public bool isCastleMove;
        public int moveID;

        public Move(int[] _start, int[] _end, string[][] _board,
            bool _isEnPassant = false, bool _isPawnPromotion = false, bool _isCastleMove = false)
        {
            this.start = _start;
            this.end = _end;
            this.pieceMoved = _board[start[0]][start[1]];
            this.pieceCaptured = _board[end[0]][end[1]];
            this.isEnPassant = _isEnPassant;
            this.isPawnPromotion = _isPawnPromotion;
            this.isCastleMove = _isCastleMove;
            if (this.isEnPassant)
            {
                this.pieceCaptured = this.pieceMoved == "wp" ? "bP" : "wP";
            }
            this.moveID = start[0] * 1000 + start[1] * 100 + end[0] * 10 + end[1];
        }

        public override bool Equals(object? obj)
        {
            if (obj is Move)
            {
                Move move = (Move)obj;
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
