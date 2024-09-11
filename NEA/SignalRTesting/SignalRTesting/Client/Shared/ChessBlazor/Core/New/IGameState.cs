using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public interface IGameState
    {
        public Board board { get; }
        public bool whiteToMove { get; set; }
        public List<Move> moveLog { get; }
        public (int, int) wkLocation { get; set; }
        public (int, int) bkLocation { get; set; }
        public bool checkmate { get; set; }
        public bool stalemate { get; set; }
        public bool inCheck { get; set; }
        public List<(int, int, int, int)?> pins { get; set; }
        public List<(int, int, int, int)?> checks { get; set; }
        public (int, int)? enpassantPossible { get; set; }
        public List<(int, int)?> enpassantPossibleLog { get; }
        public (bool wks, bool bks, bool wqs, bool bqs) currentCastlingRights { get; set; }
        public List<(bool wks, bool bks, bool wqs, bool bqs)> castleRightsLog { get; }

        public void MakeMove(Move move);
        public void UndoMove();
        public void UpdateCastleRights(Move move);
        public List<Move> GetValidMoves();
        public bool InCheck();
        public bool SquareUnderAttack(int row, int col);
        public List<Move> GetAllPossibleMoves();
        public (bool _inCheck, List<(int, int, int, int)?> _pins, List<(int, int, int, int)?> _checks)
            CheckForPinsAndChecks();
    }
}
