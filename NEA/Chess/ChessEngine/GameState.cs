using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine
{
    public class GameState
    {
        public List<List<string>> board = new List<List<string>>
        {
            new List<string> { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" },
            new List<string> { "bP", "bP", "bP", "bP", "bP", "bP", "bP", "bP" },
            new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },          
            new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },          
            new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },          
            new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },          
            new List<string> { "wP", "wP", "wP", "wP", "wP", "wP", "wP", "wP" },            
            new List<string> { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }      
        };
        public Dictionary<char, Func<int, int, List<Move>>> moveFunctions = new Dictionary<char, Func<int, int, List<Move>>>();
        public bool whiteToMove = true;
        public List<Move> moveLog = new List<Move>();
        public List<int> whiteKingLocation = new List<int> { 7, 4 };
        public List<int> blackKingLocation = new List<int> { 0, 4 };
        public bool inCheck = false;
        public List<List<int>> pins = new List<List<int>>();
        public List<List<int>> checks = new List<List<int>>();
        public bool checkMate = false;
        public bool staleMate = false;
        public List<int>? enPassantPossible = null;
        public CastleRights currentCastlingRight = new CastleRights(true, true, true, true);
        public List<CastleRights> castleRightsLog;

        public GameState()
        {
            this.castleRightsLog = new List<CastleRights> { new CastleRights(
                currentCastlingRight.wks,
                currentCastlingRight.bks,
                currentCastlingRight.wqs,
                currentCastlingRight.bqs) };
            moveFunctions.Add('P', getPawnMoves);
            moveFunctions.Add('R', getRookMoves);
            moveFunctions.Add('N', getKnightMoves);
            moveFunctions.Add('B', getBishopMoves);
            moveFunctions.Add('Q', getQueenMoves);
            moveFunctions.Add('K', getKingMoves);
        }

        public void makeMove(Move move)
        {
            board[move.startRow][move.startCol] = "--";
            board[move.endRow][move.endCol] = move.pieceMoved;
            moveLog.Add(move);
            whiteToMove = !whiteToMove;

            if (move.pieceMoved == "wK")
            {
                whiteKingLocation = new List<int> { move.endRow, move.endCol };
            }
            else if (move.pieceMoved == "bK")
            {
                blackKingLocation = new List<int> { move.endRow, move.endCol };
            }

            if (move.pieceMoved[1] == 'P' && Math.Abs(move.startRow - move.endRow) == 2)
            {
                enPassantPossible = new List<int> { (move.endRow + move.startRow) / 2, move.endCol };
            }
            else
            {
                enPassantPossible = null;
            }

            if (move.enPassant)
            {
                board[move.startRow][move.endCol] = "--";
            }

            if (move.pawnPromotion)
            {
                board[move.endRow][move.endCol] = move.pieceMoved[0].ToString() + "Q";
            }

            if (move.isCastleMove)
            {
                if (move.endCol - move.startCol == 2)
                {
                    board[move.endRow][move.endCol - 1] = board[move.endRow][move.endCol + 1];
                    board[move.endRow][move.endCol + 1] = "--";
                }
                else
                {
                    board[move.endRow][move.endCol + 1] = board[move.endRow][move.endCol - 2];
                    board[move.endRow][move.endCol - 2] = "--";
                }
            }

            updateCastleRights(move);
            castleRightsLog.Add(new CastleRights(
                currentCastlingRight.wks,
                currentCastlingRight.bks,
                currentCastlingRight.wqs,
                currentCastlingRight.bqs));
        }

        public void updateCastleRights(Move move)
        {
            if (move.pieceMoved == "wK")
            {
                currentCastlingRight.wks = false;
                currentCastlingRight.wqs = false;
            }
            else if (move.pieceMoved == "bK")
            {
                currentCastlingRight.bks = false;
                currentCastlingRight.bqs = false;
            }
            else if (move.pieceMoved == "wR")
            {
                if (move.startRow == 7)
                {
                    if (move.startCol == 0)
                    {
                        currentCastlingRight.wqs = false;
                    }
                    else if (move.startCol == 7)
                    {
                        currentCastlingRight.wks = false;
                    }
                }
            }
            else if (move.pieceMoved == "bR")
            {
                if (move.startRow == 0)
                {
                    if (move.startCol == 0)
                    {
                        currentCastlingRight.bqs = false;
                    }
                    else if (move.startCol == 7)
                    {
                        currentCastlingRight.bks = false;
                    }
                }
            }
        }

        public void UndoMove()
        {
            if (moveLog.Count != 0)
            {
                Move move = moveLog[moveLog.Count - 1];
                moveLog.RemoveAt(moveLog.Count - 1);
                board[move.startRow][move.startCol] = move.pieceMoved;
                board[move.endRow][move.endCol] = move.pieceCaptured;
                whiteToMove = !whiteToMove;

                if (move.pieceMoved == "wK")
                {
                    whiteKingLocation = new List<int> { move.startRow, move.startCol };
                }
                else if (move.pieceMoved == "bK")
                {
                    blackKingLocation = new List<int> { move.startRow, move.startCol };
                }

                if (move.enPassant)
                {
                    board[move.endRow][move.endCol] = "--";
                    board[move.startRow][move.endCol] = move.pieceCaptured;
                    enPassantPossible = new List<int> { move.endRow, move.endCol };
                }

                if (move.pieceMoved[1] == 'P' && Math.Abs(move.startRow - move.endRow) == 2)
                {
                    enPassantPossible = null;
                }

                castleRightsLog.RemoveAt(castleRightsLog.Count - 1);
                var newRights = castleRightsLog[castleRightsLog.Count - 1];
                currentCastlingRight = new CastleRights(newRights.wks, newRights.bks, newRights.wqs, newRights.bqs);

                if (move.isCastleMove)
                {
                    if (move.endCol - move.startCol == 2)
                    {
                        board[move.endRow][move.endCol + 1] = board[move.endRow][move.endCol - 1];
                        board[move.endRow][move.endCol - 1] = "--";
                    }
                    else
                    {
                        board[move.endRow][move.endCol - 2] = board[move.endRow][move.endCol + 1];
                        board[move.endRow][move.endCol + 1] = "--";
                    }
                }
            }
        }

        public List<Move> getValidMoves()
        {
            CastleRights tempCastleRights = new CastleRights(
                currentCastlingRight.wks,
                currentCastlingRight.bks,
                currentCastlingRight.wqs,
                currentCastlingRight.bqs);
            List<Move> moves = new List<Move>();
            checkForPinsAndChecks(out inCheck, out pins, out checks);

            int kRow;
            int kCol;

            if (whiteToMove)
            {
                kRow = whiteKingLocation[0];
                kCol = whiteKingLocation[1];
            }
            else
            {
                kRow = blackKingLocation[0];
                kCol = blackKingLocation[1];
            }

            if (inCheck)
            {
                if (checks.Count == 1)
                {
                    moves = getAllPossibleMoves();

                    var check = checks[0];
                    int checkRow = check[0];
                    int checkCol = check[1];

                    char pieceChecking = board[checkRow][checkCol][1];
                    List<List<int>> validSquares = new List<List<int>>();

                    if (pieceChecking == 'N')
                    {
                        validSquares = new List<List<int>> { new List<int> { checkRow, checkCol } };
                    }
                    else
                    {
                        for (int i = 1; i < 8; i++)
                        {
                            var validSquare = new List<int> { kRow + check[2] * i, kCol + check[3] * i };
                            validSquares.Add(validSquare);

                            if (validSquare[0] == checkRow && validSquare[1] == checkCol)
                            {
                                break;
                            }
                        }
                    }

                    for (int i = moves.Count - 1; i >= 0; i--)
                    {
                        if (moves[i].pieceMoved[1] != 'K')
                        {
                            if (!validSquares.Any(s => s.SequenceEqual(new List<int> { moves[i].endRow, moves[i].endCol })))
                            {
                                moves.RemoveAt(i);
                            }
                        }
                    }
                }
                else
                {
                    moves = getKingMoves(kRow, kCol);
                }
            }
            else
            {
                moves = getAllPossibleMoves();
            }

            if (moves.Count == 0)
            {
                if (inCheck)
                {
                    checkMate = true;
                }
                else
                {
                    staleMate = true;
                }
            }
            else
            {
                checkMate = false;
                staleMate = false;
            }

            if (whiteToMove)
            {
                moves.AddRange(getCastleMoves(whiteKingLocation[0], whiteKingLocation[1]));
            }
            else
            {
                moves.AddRange(getCastleMoves(blackKingLocation[0], blackKingLocation[1]));
            }

            currentCastlingRight = tempCastleRights;
            return moves;
        }

        public bool squareUnderAttack(int row, int col)
        {
            whiteToMove = !whiteToMove;
            List<Move> opponentMoves = getAllPossibleMoves();
            whiteToMove = !whiteToMove;
            foreach(Move move in opponentMoves)
            {
                if (move.endRow == row && move.endCol == col)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Move> getAllPossibleMoves()
        {
            var moves = new List<Move>();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var turn = board[r][c][0];
                    if ((turn == 'w' && whiteToMove) || (turn == 'b' && !whiteToMove))
                    {
                        var piece = board[r][c][1];
                        moves.AddRange(moveFunctions[piece](r, c));
                    }
                }
            }
            return moves;
        }

        public List<Move> getPawnMoves(int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = pins.Count - 1; i >= 0; i--)
            {
                if (pins[i][0] == r && pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { pins[i][2], pins[i][3] };
                    pins.RemoveAt(i);
                    break;
                }
            }

            var moveAmount = whiteToMove ? -1 : 1;
            var startRow = whiteToMove ? 6 : 1;
            var backRow = whiteToMove ? 0 : 7;
            var enemyColour = whiteToMove ? 'b' : 'w';
            var pawnPromotion = false;

            if (board[r + moveAmount][c] == "--")
            {
                if (!piecePinned || pinDirection[0] == moveAmount && pinDirection[1] == 0)
                {
                    if (r + moveAmount == backRow)
                    {
                        pawnPromotion = true;
                    }
                    moves.Add(new Move(new List<int> { r, c }, new List<int> { r + moveAmount, c }, board, pawnPromotion: pawnPromotion));
                    if (r == startRow && board[r + 2 * moveAmount][c] == "--")
                    {
                        moves.Add(new Move(new List<int> { r, c }, new List<int> { r + 2 * moveAmount, c }, board));
                    }
                }
            }
            if (c - 1 >= 0)
            {
                if (!piecePinned || pinDirection[0] == moveAmount && pinDirection[1] == -1)
                {
                    if (board[r + moveAmount][c - 1][0] == enemyColour)
                    {
                        if (r + moveAmount == backRow)
                        {
                            pawnPromotion = true;
                        }
                        moves.Add(new Move(new List<int> { r, c }, new List<int> { r + moveAmount, c - 1 }, board, pawnPromotion: pawnPromotion));
                    }
                    if (enPassantPossible != null && enPassantPossible[0] == r + moveAmount && enPassantPossible[1] == c - 1)
                    {
                        moves.Add(new Move(new List<int> { r, c }, new List<int> { r + moveAmount, c - 1 }, board, enPassant: true));
                    }
                }
            }
            if (c + 1 < 8)
            {
                if (!piecePinned || pinDirection[0] == moveAmount && pinDirection[1] == 1)
                {
                    if (board[r + moveAmount][c + 1][0] == enemyColour)
                    {
                        if (r + moveAmount == backRow)
                        {
                            pawnPromotion = true;
                        }
                        moves.Add(new Move(new List<int> { r, c }, new List<int> { r + moveAmount, c + 1 }, board, pawnPromotion: pawnPromotion));
                    }
                    if (enPassantPossible != null && enPassantPossible[0] == r + moveAmount && enPassantPossible[1] == c + 1)
                    {
                        moves.Add(new Move(new List<int> { r, c }, new List<int> { r + moveAmount, c + 1 }, board, enPassant: true));
                    }
                }
            }
            return moves;
        }

        public List<Move> getRookMoves(int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = pins.Count - 1; i >= 0; i--)
            {
                if (pins[i][0] == r && pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { pins[i][2], pins[i][3] };
                    if (board[r][c][1] != 'Q')
                    {
                        pins.RemoveAt(i);
                    }
                    break;
                }

            }

            var directions = new List<List<int>> { 
                new List<int> { -1, 0 }, new List<int> { 1, 0 }, new List<int> { 0, -1 }, new List<int> { 0, 1 } };
            var enemyColour = whiteToMove ? 'b' : 'w';

            foreach (var direction in directions)
            {
                for (int i = 1; i < 8; i++)
                {
                    var endRow = r + direction[0] * i;
                    var endCol = c + direction[1] * i;
                    if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                    {
                        if (!piecePinned || (pinDirection[0] == direction[0] && pinDirection[1] == direction[1]) ||
                        (pinDirection[0] == -direction[0] && pinDirection[1] == -direction[1]))
                        {
                            if (board[endRow][endCol] == "--")
                            {
                                moves.Add(new Move(new List<int> { r, c }, new List<int> { endRow, endCol }, board));
                            }
                            else if (board[endRow][endCol][0] == enemyColour)
                            {
                                moves.Add(new Move(new List<int> { r, c }, new List<int> { endRow, endCol }, board));
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }                    
                }
            }
            return moves;
        }

        public List<Move> getKnightMoves(int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = pins.Count - 1; i >= 0; i--)
            {
                if (pins[i][0] == r && pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { pins[i][2], pins[i][3] };
                    pins.RemoveAt(i);
                    break;
                }
            }

            var directions = new List<List<int>> {
                new List<int> { -2, -1 }, new List<int> { -2, 1 }, new List<int> { -1, -2 }, new List<int> { -1, 2 },
                new List<int> { 1, -2 }, new List<int> { 1, 2 }, new List<int> { 2, -1 }, new List<int> { 2, 1 } };
            var enemyColour = whiteToMove ? 'b' : 'w';

            foreach (var direction in directions)
            {
                var endRow = r + direction[0];
                var endCol = c + direction[1];
                if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                {
                    if (!piecePinned)
                    {
                        if (board[endRow][endCol] == "--" || board[endRow][endCol][0] == enemyColour)
                        {
                            moves.Add(new Move(new List<int> { r, c }, new List<int> { endRow, endCol }, board));
                        }
                    }
                }
            }

            return moves;
        }

        public List<Move> getBishopMoves(int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = pins.Count - 1; i >= 0; i--)
            {
                if (pins[i][0] == r && pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { pins[i][2], pins[i][3] };
                    pins.RemoveAt(i);
                    break;
                }
            }

            var directions = new List<List<int>> {
                new List<int> { -1, -1 }, new List<int> { -1, 1 }, new List<int> { 1, -1 }, new List<int> { 1, 1 } };
            var enemyColour = whiteToMove ? 'b' : 'w';

            foreach (var direction in directions)
            {
                for (int i = 1; i < 8; i++)
                {
                    var endRow = r + direction[0] * i;
                    var endCol = c + direction[1] * i;
                    if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                    {
                        if (!piecePinned || (pinDirection[0] == direction[0] && pinDirection[1] == direction[1]) ||
                        (pinDirection[0] == -direction[0] && pinDirection[1] == -direction[1]))
                        {
                            if (board[endRow][endCol] == "--")
                            {
                                moves.Add(new Move(new List<int> { r, c }, new List<int> { endRow, endCol }, board));
                            }
                            else if (board[endRow][endCol][0] == enemyColour)
                            {
                                moves.Add(new Move(new List<int> { r, c }, new List<int> { endRow, endCol }, board));
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return moves;
        }

        public List<Move> getQueenMoves(int r, int c)
        {
            var moves = new List<Move>();
            moves.AddRange(getRookMoves(r, c));
            moves.AddRange(getBishopMoves(r, c));
            return moves;
        }

        public List<Move> getKingMoves(int r, int c)
        {
            var moves = new List<Move>();
            var rowMoves = new List<int> { -1, -1, -1, 0, 0, 1, 1, 1 };
            var colMoves = new List<int> { -1, 0, 1, -1, 1, -1, 0, 1 };
            var allyColour = whiteToMove ? 'w' : 'b';

            for (int i = 0; i < 8; i++)
            {
                var endRow = r + rowMoves[i];
                var endCol = c + colMoves[i];
                if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                {
                    var endPiece = board[endRow][endCol];
                    if (endPiece[0] != allyColour)
                    {
                        if (allyColour == 'w')
                        {
                            whiteKingLocation = new List<int> { endRow, endCol };
                        }
                        else
                        {
                            blackKingLocation = new List<int> { endRow, endCol };
                        }
                        bool _inCheck;
                        List<List<int>> _pins, _checks;
                        checkForPinsAndChecks(out _inCheck, out _pins, out _checks);
                        if (!_inCheck)
                        {
                            moves.Add(new Move(new List<int> { r, c }, new List<int> { endRow, endCol }, board));
                        }
                        if (allyColour == 'w')
                        {
                            whiteKingLocation = new List<int> { r, c };
                        }
                        else
                        {
                            blackKingLocation = new List<int> { r, c };
                        }
                    }
                }
            }
            return moves;
        }

        public void checkForPinsAndChecks(out bool _inCheck, out List<List<int>> _pins, out List<List<int>> _checks)
        {
            var pins = new List<List<int>>();
            var checks = new List<List<int>>();
            var inCheck = false;

            var enemyColour = whiteToMove ? 'b' : 'w';
            var allyColour = whiteToMove ? 'w' : 'b';
            var startRow = whiteToMove ? whiteKingLocation[0] : blackKingLocation[0];
            var startCol = whiteToMove ? whiteKingLocation[1] : blackKingLocation[1];

            var directions = new List<List<int>> {
                new List<int> { -1, 0 }, new List<int> { 0, -1 }, new List<int> { 1, 0 }, new List<int> { 0, 1 },
                new List<int> { -1, -1 }, new List<int> { -1, 1 }, new List<int> { 1, -1 }, new List<int> { 1, 1 } };

            for (int j = 0; j < directions.Count(); j++)
            {
                var d = directions[j];
                var possiblePin = new List<int>();
                for (int i = 1; i < 8; i++)
                {
                    var endRow = startRow + d[0] * i;
                    var endCol = startCol + d[1] * i;

                    if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                    {
                        var endPiece = board[endRow][endCol];
                        if (endPiece[0] == allyColour && endPiece[1] != 'K')
                        {
                            if (possiblePin.Count == 0)
                            {
                                possiblePin = new List<int> { endRow, endCol, d[0], d[1] };
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (endPiece[0] == enemyColour)
                        {
                            var type = endPiece[1];
                            if ((0 <= j && j <= 3 && type == 'R') || 
                                (4 <= j && j <= 7 && type == 'B') ||
                                (i == 1 && type == 'P' && ((enemyColour == 'b' && 4 <= j && j <= 5) || (enemyColour == 'w' && 6 <= j && j<= 7))) ||
                                (type == 'Q') || 
                                (i == 1 && type == 'K'))
                            {
                                if (possiblePin.Count == 0)
                                {
                                    inCheck = true;
                                    checks.Add(new List<int> { endRow, endCol, d[0], d[1] });
                                    break;
                                }
                                else
                                {
                                    pins.Add(possiblePin);
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var knightMoves = new List<List<int>> {
                new List<int> { -2, -1 }, new List<int> { -2, 1 }, new List<int> { -1, -2 }, new List<int> { -1, 2 },
                new List<int> { 1, -2 }, new List<int> { 1, 2 }, new List<int> { 2, -1 }, new List<int> { 2, 1 } };

            foreach (var move in knightMoves)
            {
                var endRow = startRow + move[0];
                var endCol = startCol + move[1];
                if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                {
                    var endPiece = board[endRow][endCol];
                    if (endPiece[0] == enemyColour && endPiece[1] == 'N')
                    {
                        inCheck = true;
                        checks.Add(new List<int> { endRow, endCol, move[0], move[1] });
                    }
                }
            }

            _inCheck = inCheck;
            _pins = pins;
            _checks = checks;
        }

        public List<Move> getCastleMoves(int r, int c)
        {
            if (squareUnderAttack(r, c))
            {
                return new List<Move>();
            }
            var moves = new List<Move>();
            if ((whiteToMove && currentCastlingRight.wks) || (!whiteToMove && currentCastlingRight.bks))
            {
                moves.AddRange(getKingsideCastleMoves(r, c));
            }
            if ((whiteToMove && currentCastlingRight.wqs) || (!whiteToMove && currentCastlingRight.bqs))
            {
                moves.AddRange(getQueensideCastleMoves(r, c));
            }
            return moves;
        }

        public List<Move> getKingsideCastleMoves(int r, int c)
        {
            var moves = new List<Move>();
            if (board[r][c + 1] == "--" && board[r][c+2] == "--")
            {
                if (!squareUnderAttack(r, c + 1) && !squareUnderAttack(r, c + 2))
                {
                    moves.Add(new Move(new List<int> { r, c }, new List<int> { r, c + 2 }, board, isCastleMove: true));
                }
            }
            return moves;
        }

        public List<Move> getQueensideCastleMoves(int r, int c)
        {
            var moves = new List<Move>();
            if (board[r][c - 1] == "--" && board[r][c - 2] == "--" && board[r][c - 3] == "--")
            {
                if (!squareUnderAttack(r, c - 1) && !squareUnderAttack(r, c - 2))
                {
                    moves.Add(new Move(new List<int> { r, c }, new List<int> { r, c - 2 }, board, isCastleMove: true));
                }
            }
            return moves;
        }
    }
}
