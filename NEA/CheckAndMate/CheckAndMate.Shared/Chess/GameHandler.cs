using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public static class GameHandler
    {
        public static void MakeMove(Game game, Move move)
        {
            game.gameState.board[move.startRow][move.startCol] = "--";
            game.gameState.board[move.endRow][move.endCol] = move.pieceMoved;
            game.gameState.moveLog.Add(move);
            game.gameState.whiteToMove = !game.gameState.whiteToMove;

            if (move.pieceMoved == "wK")
            {
                game.gameState.whiteKingLocation = new List<int> { move.endRow, move.endCol };
            }
            else if (move.pieceMoved == "bK")
            {
                game.gameState.blackKingLocation = new List<int> { move.endRow, move.endCol };
            }

            if (move.pieceMoved[1] == 'P' && Math.Abs(move.startRow - move.endRow) == 2)
            {
                game.gameState.enPassantPossible = new List<int> { (move.endRow + move.startRow) / 2, move.endCol };
            }
            else
            {
                game.gameState.enPassantPossible = null;
            }

            if (move.enPassant)
            {
                game.gameState.board[move.startRow][move.endCol] = "--";
            }

            if (move.pawnPromotion)
            {
                game.gameState.board[move.endRow][move.endCol] = move.pieceMoved[0].ToString() + "Q";
            }

            if (move.isCastleMove)
            {
                if (move.endCol - move.startCol == 2)
                {
                    game.gameState.board[move.endRow][move.endCol - 1] = game.gameState.board[move.endRow][move.endCol + 1];
                    game.gameState.board[move.endRow][move.endCol + 1] = "--";
                }
                else
                {
                    game.gameState.board[move.endRow][move.endCol + 1] = game.gameState.board[move.endRow][move.endCol - 2];
                    game.gameState.board[move.endRow][move.endCol - 2] = "--";
                }
            }

            UpdateCastleRights(game, move);
            game.gameState.castleRightsLog.Add(new CastleRights(
                game.gameState.currentCastlingRight.wks,
                game.gameState.currentCastlingRight.bks,
                game.gameState.currentCastlingRight.wqs,
                game.gameState.currentCastlingRight.bqs));
        }

        public static void UndoMove(Game game)
        {
            if (game.gameState.moveLog.Count != 0)
            {
                Move move = game.gameState.moveLog[game.gameState.moveLog.Count - 1];
                game.gameState.moveLog.RemoveAt(game.gameState.moveLog.Count - 1);
                game.gameState.board[move.startRow][move.startCol] = move.pieceMoved;
                game.gameState.board[move.endRow][move.endCol] = move.pieceCaptured;
                game.gameState.whiteToMove = !game.gameState.whiteToMove;

                if (move.pieceMoved == "wK")
                {
                    game.gameState.whiteKingLocation = new List<int> { move.startRow, move.startCol };
                }
                else if (move.pieceMoved == "bK")
                {
                    game.gameState.blackKingLocation = new List<int> { move.startRow, move.startCol };
                }

                if (move.enPassant)
                {
                    game.gameState.board[move.endRow][move.endCol] = "--";
                    game.gameState.board[move.startRow][move.endCol] = move.pieceCaptured;
                    game.gameState.enPassantPossible = new List<int> { move.endRow, move.endCol };
                }

                if (move.pieceMoved[1] == 'P' && Math.Abs(move.startRow - move.endRow) == 2)
                {
                    game.gameState.enPassantPossible = null;
                }

                game.gameState.castleRightsLog.RemoveAt(game.gameState.castleRightsLog.Count - 1);
                var newRights = game.gameState.castleRightsLog[game.gameState.castleRightsLog.Count - 1];
                game.gameState.currentCastlingRight = new CastleRights(newRights.wks, newRights.bks, newRights.wqs, newRights.bqs);

                if (move.isCastleMove)
                {
                    if (move.endCol - move.startCol == 2)
                    {
                        game.gameState.board[move.endRow][move.endCol + 1] = game.gameState.board[move.endRow][move.endCol - 1];
                        game.gameState.board[move.endRow][move.endCol - 1] = "--";
                    }
                    else
                    {
                        game.gameState.board[move.endRow][move.endCol - 2] = game.gameState.board[move.endRow][move.endCol + 1];
                        game.gameState.board[move.endRow][move.endCol + 1] = "--";
                    }
                }
            }
        }

        public static void UpdateCastleRights(Game game, Move move)
        {
            if (move.pieceMoved == "wK")
            {
                game.gameState.currentCastlingRight.wks = false;
                game.gameState.currentCastlingRight.wqs = false;
            }
            else if (move.pieceMoved == "bK")
            {
                game.gameState.currentCastlingRight.bks = false;
                game.gameState.currentCastlingRight.bqs = false;
            }
            else if (move.pieceMoved == "wR")
            {
                if (move.startRow == 7)
                {
                    if (move.startCol == 0)
                    {
                        game.gameState.currentCastlingRight.wqs = false;
                    }
                    else if (move.startCol == 7)
                    {
                        game.gameState.currentCastlingRight.wks = false;
                    }
                }
            }
            else if (move.pieceMoved == "bR")
            {
                if (move.startRow == 0)
                {
                    if (move.startCol == 0)
                    {
                        game.gameState.currentCastlingRight.bqs = false;
                    }
                    else if (move.startCol == 7)
                    {
                        game.gameState.currentCastlingRight.bks = false;
                    }
                }
            }
        }

        public static List<Move> FindValidMoves(Game game)
        {
            CastleRights tempCastleRights = new CastleRights(
                game.gameState.currentCastlingRight.wks,
                game.gameState.currentCastlingRight.bks,
                game.gameState.currentCastlingRight.wqs,
                game.gameState.currentCastlingRight.bqs);
            List<Move> moves = new List<Move>();
            bool inCheck;
            var pins = new List<List<int>>();
            var checks = new List<List<int>>();
            CheckForPinsAndChecks(game, out inCheck, out pins, out checks);
            game.gameState.inCheck = inCheck;
            game.gameState.pins = pins;
            game.gameState.checks = checks;

            int kRow;
            int kCol;

            if (game.gameState.whiteToMove)
            {
                kRow = game.gameState.whiteKingLocation[0];
                kCol = game.gameState.whiteKingLocation[1];
            }
            else
            {
                kRow = game.gameState.blackKingLocation[0];
                kCol = game.gameState.blackKingLocation[1];
            }

            if (game.gameState.inCheck)
            {
                if (game.gameState.checks.Count == 1)
                {
                    moves = GetAllPossibleMoves(game);

                    var check = game.gameState.checks[0];
                    int checkRow = check[0];
                    int checkCol = check[1];

                    char pieceChecking = game.gameState.board[checkRow][checkCol][1];
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
                    moves = GetKingMoves(game, kRow, kCol);
                }
            }
            else
            {
                moves = GetAllPossibleMoves(game);
            }

            if (moves.Count == 0)
            {
                if (game.gameState.inCheck)
                {
                    game.gameState.checkMate = true;
                }
                else
                {
                    game.gameState.staleMate = true;
                }
            }
            else
            {
                game.gameState.checkMate = false;
                game.gameState.staleMate = false;
            }

            if (game.gameState.whiteToMove)
            {
                moves.AddRange(GetCastleMoves(game, game.gameState.whiteKingLocation[0], game.gameState.whiteKingLocation[1]));
            }
            else
            {
                moves.AddRange(GetCastleMoves(game, game.gameState.blackKingLocation[0], game.gameState.blackKingLocation[1]));
            }

            game.gameState.currentCastlingRight = tempCastleRights;
            return moves;
        }

        public static void CheckForPinsAndChecks(Game game, out bool _inCheck, out List<List<int>> _pins, out List<List<int>> _checks)
        {
            var pins = new List<List<int>>();
            var checks = new List<List<int>>();
            var inCheck = false;

            var enemyColour = game.gameState.whiteToMove ? 'b' : 'w';
            var allyColour = game.gameState.whiteToMove ? 'w' : 'b';
            var startRow = game.gameState.whiteToMove ? game.gameState.whiteKingLocation[0] : game.gameState.blackKingLocation[0];
            var startCol = game.gameState.whiteToMove ? game.gameState.whiteKingLocation[1] : game.gameState.blackKingLocation[1];

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
                        var endPiece = game.gameState.board[endRow][endCol];
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
                                (i == 1 && type == 'P' && ((enemyColour == 'b' && 4 <= j && j <= 5) || (enemyColour == 'w' && 6 <= j && j <= 7))) ||
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
                    var endPiece = game.gameState.board[endRow][endCol];
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

        public static bool SquareUnderAttack(Game game, int row, int col)
        {
            game.gameState.whiteToMove = !game.gameState.whiteToMove;
            List<Move> opponentMoves = GetAllPossibleMoves(game);
            game.gameState.whiteToMove = !game.gameState.whiteToMove;
            foreach (Move move in opponentMoves)
            {
                if (move.endRow == row && move.endCol == col)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Move> GetAllPossibleMoves(Game game)
        {
            var moves = new List<Move>();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var turn = game.gameState.board[r][c][0];
                    if ((turn == 'w' && game.gameState.whiteToMove) || (turn == 'b' && !game.gameState.whiteToMove))
                    {
                        var piece = game.gameState.board[r][c][1];
                        moves.AddRange(GetMoveFunction(piece.ToString())(game, r, c));
                    }
                }
            }
            return moves;
        }

        public static Func<Game, int, int, List<Move>> GetMoveFunction(string piece)
        {
            var moveFunctions = new Dictionary<string, Func<Game, int, int, List<Move>>>
            {
                { "P", GetPawnMoves }, { "R", GetRookMoves }, { "N", GetKnightMoves },
                { "B", GetBishopMoves }, { "Q", GetQueenMoves }, { "K", GetKingMoves }
            };
            return moveFunctions[piece];
        }

        public static List<Move> GetPawnMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = game.gameState.pins.Count - 1; i >= 0; i--)
            {
                if (game.gameState.pins[i][0] == r && game.gameState.pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { game.gameState.pins[i][2], game.gameState.pins[i][3] };
                    game.gameState.pins.RemoveAt(i);
                    break;
                }
            }

            var moveAmount = game.gameState.whiteToMove ? -1 : 1;
            var startRow = game.gameState.whiteToMove ? 6 : 1;
            var backRow = game.gameState.whiteToMove ? 0 : 7;
            var enemyColour = game.gameState.whiteToMove ? 'b' : 'w';
            var pawnPromotion = false;

            if (game.gameState.board[r + moveAmount][c] == "--")
            {
                if (!piecePinned || pinDirection[0] == moveAmount && pinDirection[1] == 0)
                {
                    if (r + moveAmount == backRow)
                    {
                        pawnPromotion = true;
                    }
                    moves.Add(new Move(r, c, r + moveAmount, c, game.gameState.board, pawnPromotion: pawnPromotion));
                    if (r == startRow && game.gameState.board[r + 2 * moveAmount][c] == "--")
                    {
                        moves.Add(new Move(r, c, r + 2 * moveAmount, c, game.gameState.board));
                    }
                }
            }
            if (c - 1 >= 0)
            {
                if (!piecePinned || pinDirection[0] == moveAmount && pinDirection[1] == -1)
                {
                    if (game.gameState.board[r + moveAmount][c - 1][0] == enemyColour)
                    {
                        if (r + moveAmount == backRow)
                        {
                            pawnPromotion = true;
                        }
                        moves.Add(new Move(r, c, r + moveAmount, c - 1, game.gameState.board, pawnPromotion: pawnPromotion));
                    }
                    if (game.gameState.enPassantPossible != null && game.gameState.enPassantPossible[0] == r + moveAmount && game.gameState.enPassantPossible[1] == c - 1)
                    {
                        moves.Add(new Move(r, c, r + moveAmount, c - 1, game.gameState.board, enPassant: true));
                    }
                }
            }
            if (c + 1 < 8)
            {
                if (!piecePinned || pinDirection[0] == moveAmount && pinDirection[1] == 1)
                {
                    if (game.gameState.board[r + moveAmount][c + 1][0] == enemyColour)
                    {
                        if (r + moveAmount == backRow)
                        {
                            pawnPromotion = true;
                        }
                        moves.Add(new Move(r, c, r + moveAmount, c + 1, game.gameState.board, pawnPromotion: pawnPromotion));
                    }
                    if (game.gameState.enPassantPossible != null && game.gameState.enPassantPossible[0] == r + moveAmount && game.gameState.enPassantPossible[1] == c + 1)
                    {
                        moves.Add(new Move(r, c, r + moveAmount, c + 1, game.gameState.board, enPassant: true));
                    }
                }
            }
            return moves;
        }

        public static List<Move> GetRookMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = game.gameState.pins.Count - 1; i >= 0; i--)
            {
                if (game.gameState.pins[i][0] == r && game.gameState.pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { game.gameState.pins[i][2], game.gameState.pins[i][3] };
                    if (game.gameState.board[r][c][1] != 'Q')
                    {
                        game.gameState.pins.RemoveAt(i);
                    }
                    break;
                }

            }

            var directions = new List<List<int>> {
                new List<int> { -1, 0 }, new List<int> { 1, 0 }, new List<int> { 0, -1 }, new List<int> { 0, 1 } };
            var enemyColour = game.gameState.whiteToMove ? 'b' : 'w';

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
                            if (game.gameState.board[endRow][endCol] == "--")
                            {
                                moves.Add(new Move(r, c, endRow, endCol, game.gameState.board));
                            }
                            else if (game.gameState.board[endRow][endCol][0] == enemyColour)
                            {
                                moves.Add(new Move(r, c, endRow, endCol, game.gameState.board));
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

        public static List<Move> GetKnightMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = game.gameState.pins.Count - 1; i >= 0; i--)
            {
                if (game.gameState.pins[i][0] == r && game.gameState.pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { game.gameState.pins[i][2], game.gameState.pins[i][3] };
                    game.gameState.pins.RemoveAt(i);
                    break;
                }
            }

            var directions = new List<List<int>> {
                new List<int> { -2, -1 }, new List<int> { -2, 1 }, new List<int> { -1, -2 }, new List<int> { -1, 2 },
                new List<int> { 1, -2 }, new List<int> { 1, 2 }, new List<int> { 2, -1 }, new List<int> { 2, 1 } };
            var enemyColour = game.gameState.whiteToMove ? 'b' : 'w';

            foreach (var direction in directions)
            {
                var endRow = r + direction[0];
                var endCol = c + direction[1];
                if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                {
                    if (!piecePinned)
                    {
                        if (game.gameState.board[endRow][endCol] == "--" || game.gameState.board[endRow][endCol][0] == enemyColour)
                        {
                            moves.Add(new Move(r, c, endRow, endCol, game.gameState.board));
                        }
                    }
                }
            }

            return moves;
        }

        public static List<Move> GetBishopMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            var piecePinned = false;
            var pinDirection = new List<int>();

            for (int i = game.gameState.pins.Count - 1; i >= 0; i--)
            {
                if (game.gameState.pins[i][0] == r && game.gameState.pins[i][1] == c)
                {
                    piecePinned = true;
                    pinDirection = new List<int> { game.gameState.pins[i][2], game.gameState.pins[i][3] };
                    game.gameState.pins.RemoveAt(i);
                    break;
                }
            }

            var directions = new List<List<int>> {
                new List<int> { -1, -1 }, new List<int> { -1, 1 }, new List<int> { 1, -1 }, new List<int> { 1, 1 } };
            var enemyColour = game.gameState.whiteToMove ? 'b' : 'w';

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
                            if (game.gameState.board[endRow][endCol] == "--")
                            {
                                moves.Add(new Move(r, c, endRow, endCol, game.gameState.board));
                            }
                            else if (game.gameState.board[endRow][endCol][0] == enemyColour)
                            {
                                moves.Add(new Move(r, c, endRow, endCol, game.gameState.board));
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

        public static List<Move> GetQueenMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            moves.AddRange(GetRookMoves(game, r, c));
            moves.AddRange(GetBishopMoves(game, r, c));
            return moves;
        }

        public static List<Move> GetKingMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            var rowMoves = new List<int> { -1, -1, -1, 0, 0, 1, 1, 1 };
            var colMoves = new List<int> { -1, 0, 1, -1, 1, -1, 0, 1 };
            var allyColour = game.gameState.whiteToMove ? 'w' : 'b';

            for (int i = 0; i < 8; i++)
            {
                var endRow = r + rowMoves[i];
                var endCol = c + colMoves[i];
                if (endRow >= 0 && endRow < 8 && endCol >= 0 && endCol < 8)
                {
                    var endPiece = game.gameState.board[endRow][endCol];
                    if (endPiece[0] != allyColour)
                    {
                        if (allyColour == 'w')
                        {
                            game.gameState.whiteKingLocation = new List<int> { endRow, endCol };
                        }
                        else
                        {
                            game.gameState.blackKingLocation = new List<int> { endRow, endCol };
                        }
                        bool _inCheck;
                        List<List<int>> _pins, _checks;
                        CheckForPinsAndChecks(game, out _inCheck, out _pins, out _checks);
                        if (!_inCheck)
                        {
                            moves.Add(new Move(r, c, endRow, endCol, game.gameState.board));
                        }
                        if (allyColour == 'w')
                        {
                            game.gameState.whiteKingLocation = new List<int> { r, c };
                        }
                        else
                        {
                            game.gameState.blackKingLocation = new List<int> { r, c };
                        }
                    }
                }
            }
            return moves;
        }

        public static List<Move> GetCastleMoves(Game game, int r, int c)
        {
            if (SquareUnderAttack(game, r, c))
            {
                return new List<Move>();
            }
            var moves = new List<Move>();
            if ((game.gameState.whiteToMove && game.gameState.currentCastlingRight.wks) || (!game.gameState.whiteToMove && game.gameState.currentCastlingRight.bks))
            {
                moves.AddRange(GetKingsideCastleMoves(game, r, c));
            }
            if ((game.gameState.whiteToMove && game.gameState.currentCastlingRight.wqs) || (!game.gameState.whiteToMove && game.gameState.currentCastlingRight.bqs))
            {
                moves.AddRange(GetQueensideCastleMoves(game, r, c));
            }
            return moves;
        }

        public static List<Move> GetKingsideCastleMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            if (game.gameState.board[r][c + 1] == "--" && game.gameState.board[r][c + 2] == "--")
            {
                if (!SquareUnderAttack(game, r, c + 1) && !SquareUnderAttack(game, r, c + 2))
                {
                    moves.Add(new Move(r, c, r, c + 2, game.gameState.board, isCastleMove: true));
                }
            }
            return moves;
        }

        public static List<Move> GetQueensideCastleMoves(Game game, int r, int c)
        {
            var moves = new List<Move>();
            if (game.gameState.board[r][c - 1] == "--" && game.gameState.board[r][c - 2] == "--" && game.gameState.board[r][c - 3] == "--")
            {
                if (!SquareUnderAttack(game, r, c - 1) && !SquareUnderAttack(game, r, c - 2))
                {
                    moves.Add(new Move(r, c, r, c - 2, game.gameState.board, isCastleMove: true));
                }
            }
            return moves;
        }
    }
}
