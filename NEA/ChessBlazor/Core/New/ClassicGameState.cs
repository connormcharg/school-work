using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public class ClassicGameState : IGameState
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
        
        public ClassicGameState()
        {
            this.board = new Board(
                new string[][]
                {
                    new string[] { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" },
                    new string[] { "bP", "bP", "bP", "bP", "bP", "bP", "bP", "bP" },
                    new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                    new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                    new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                    new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                    new string[] { "wP", "wP", "wP", "wP", "wP", "wP", "wP", "wP" },
                    new string[] { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }
                },
                new Dictionary<char, string>
                {
                    {'P', "Pawn"}, {'R', "Rook"}, {'N', "Knight"}, 
                    {'B', "Bishop"}, {'Q', "Queen"}, {'K', "King"}, 
                    {'-', "Empty"}
                });
            this.whiteToMove = true;
            this.moveLog = new();
            this.wkLocation = (7, 4);
            this.bkLocation = (0, 4);
            this.checkmate = false;
            this.stalemate = false;
            this.inCheck = false;
            this.pins = new();
            this.checks = new();
            this.enpassantPossible = null;
            this.enpassantPossibleLog = new();
            this.enpassantPossibleLog.Add(this.enpassantPossible);
            this.currentCastlingRights = (true, true, true, true);
            this.castleRightsLog = new();
            this.castleRightsLog.Add((
                currentCastlingRights.wks,
                currentCastlingRights.bks, 
                currentCastlingRights.wqs,
                currentCastlingRights.bqs));
        }

        public void MakeMove(Move move)
        {
            this.board[move.start] = new Empty();
            this.board[move.end] = move.pieceMoved;
            this.moveLog.Add(move);
            this.whiteToMove = !this.whiteToMove;

            if (move.pieceMoved is King)
            {
                if (move.pieceMoved.isWhite)
                {
                    this.wkLocation = move.end;
                }
                else
                {
                    this.bkLocation = move.end;
                }
            }

            if (move.isPawnPromotion)
            {
                this.board[move.end] = new Queen(move.pieceMoved.isWhite);
            }

            if (move.isEnPassant)
            {
                this.board[(move.start.row, move.end.col)] = new Empty();
            }

            if (move.pieceMoved is Pawn && Math.Abs(move.start.row - move.end.row) == 2)
            {
                this.enpassantPossible = ((move.start.row + move.start.col) / 2, move.start.col);
            }
            else
            {
                this.enpassantPossible = null;
            }

            if (move.isCastle)
            {
                if (move.end.col - move.start.col == 2)
                {
                    this.board[(move.end.row, move.end.col - 1)] = this.board[(move.end.row, move.end.col + 1)];
                    this.board[(move.end.row, move.end.col + 1)] = new Empty();
                }
                else
                {
                    this.board[(move.end.row, move.end.col + 1)] = this.board[(move.end.row, move.end.col - 2)];
                    this.board[(move.end.row, move.end.col - 2)] = new Empty();
                }
            }

            this.enpassantPossibleLog.Add(this.enpassantPossible);

            this.UpdateCastleRights(move);
            this.castleRightsLog.Add((
                currentCastlingRights.wks,
                currentCastlingRights.bks,
                currentCastlingRights.wqs,
                currentCastlingRights.bqs));
        }

        public void UndoMove()
        {
            if (this.moveLog.Count != 0)
            {
                var move = this.moveLog[this.moveLog.Count - 1];
                this.moveLog.RemoveAt(this.moveLog.Count - 1);
                this.board[move.start] = move.pieceMoved;
                this.board[move.end] = move.pieceCaptured;
                this.whiteToMove = !this.whiteToMove;

                if (move.pieceMoved is King)
                {
                    if (move.pieceMoved.isWhite)
                    {
                        this.wkLocation = move.start;
                    }
                    else
                    {
                        this.bkLocation = move.start;
                    }
                }

                if (move.isEnPassant)
                {
                    this.board[move.end] = new Empty();
                    this.board[(move.start.row, move.end.col)] = move.pieceCaptured;
                }

                this.enpassantPossibleLog.RemoveAt(this.enpassantPossibleLog.Count - 1);
                this.enpassantPossible = this.enpassantPossibleLog[this.enpassantPossibleLog.Count - 1];

                this.castleRightsLog.RemoveAt(this.castleRightsLog.Count - 1);
                this.currentCastlingRights = this.castleRightsLog[this.castleRightsLog.Count - 1];

                if (move.isCastle)
                {
                    if (move.end.col - move.start.col == 2)
                    {
                        this.board[(move.end.row, move.end.col + 1)] = this.board[(move.end.row, move.end.col - 1)];
                        this.board[(move.end.row, move.end.col - 1)] = new Empty();
                    }
                    else
                    {
                        this.board[(move.end.row, move.end.col - 2)] = this.board[(move.end.row, move.end.col + 1)];
                        this.board[(move.end.row, move.end.col + 1)] = new Empty();
                    }
                }
            }
        }

        public void UpdateCastleRights(Move move)
        {

        }

        public List<Move> GetValidMoves()
        {
            return new List<Move>();
        }

        public bool InCheck()
        {
            return false;
        }

        public bool SquareUnderAttack(int row, int col)
        {
            return true;
        }

        public List<Move> GetAllPossibleMoves()
        {
            return new List<Move>();
        }

        public (bool _inCheck, List<(int, int, int, int)?> _pins, List<(int, int, int, int)?> _checks)
            CheckForPinsAndChecks()
        {
            return (false, new(), new());
        }

        public List<Move> GetCastleMoves(int row, int col)
        {
            return new();
        }
    }
}
