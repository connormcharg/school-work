namespace backend.Classes.Engine
{
    using backend.Classes.Handlers;
    using backend.Classes.State;
    using backend.Classes.Utilities;

    public class MinMaxEngine
    {
        private Random rng = new Random();

        private Dictionary<string, int> pieceScores = new Dictionary<string, int>
        {
            { "K", 0 }, { "Q", 9 }, { "R", 5 }, { "B", 3 }, { "N", 3 }, { "P", 1 }
        };

        private List<List<double>> knightScores = new List<List<double>>
        {
            new List<double> { 0.0, 0.1, 0.2, 0.2, 0.2, 0.2, 0.1, 0.0 },
            new List<double> { 0.1, 0.3, 0.5, 0.5, 0.5, 0.5, 0.3, 0.1 },
            new List<double> { 0.2, 0.5, 0.6, 0.65, 0.65, 0.6, 0.5, 0.2 },
            new List<double> { 0.2, 0.55, 0.65, 0.7, 0.7, 0.65, 0.55, 0.2 },
            new List<double> { 0.2, 0.5, 0.65, 0.7, 0.7, 0.65, 0.5, 0.2 },
            new List<double> { 0.2, 0.55, 0.6, 0.65, 0.65, 0.6, 0.55, 0.2 },
            new List<double> { 0.1, 0.3, 0.5, 0.55, 0.55, 0.5, 0.3, 0.1 },
            new List<double> { 0.0, 0.1, 0.2, 0.2, 0.2, 0.2, 0.1, 0.0 },
        };

        private List<List<double>> bishopScores = new List<List<double>>
        {
            new List<double> { 0.0, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.0 },
            new List<double> { 0.2, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4, 0.2 },
            new List<double> { 0.2, 0.4, 0.5, 0.6, 0.6, 0.5, 0.4, 0.2 },
            new List<double> { 0.2, 0.5, 0.5, 0.6, 0.6, 0.5, 0.5, 0.2 },
            new List<double> { 0.2, 0.4, 0.6, 0.6, 0.6, 0.6, 0.4, 0.2 },
            new List<double> { 0.2, 0.6, 0.6, 0.6, 0.6, 0.6, 0.6, 0.2 },
            new List<double> { 0.2, 0.5, 0.4, 0.4, 0.4, 0.4, 0.5, 0.2 },
            new List<double> { 0.0, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.0 },
        };

        private List<List<double>> rookScores = new List<List<double>>
        {
            new List<double> { 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25 },
            new List<double> { 0.5, 0.75, 0.75, 0.75, 0.75, 0.75, 0.75, 0.5 },
            new List<double> { 0.0, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.0 },
            new List<double> { 0.0, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.0 },
            new List<double> { 0.0, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.0 },
            new List<double> { 0.0, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.0 },
            new List<double> { 0.0, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.0 },
            new List<double> { 0.25, 0.25, 0.25, 0.5, 0.5, 0.25, 0.25, 0.25 },
        };

        private List<List<double>> queenScores = new List<List<double>>
        {
            new List<double> { 0.0, 0.2, 0.2, 0.3, 0.3, 0.2, 0.2, 0.0 },
            new List<double> { 0.2, 0.4, 0.4, 0.4, 0.4, 0.4, 0.4, 0.2 },
            new List<double> { 0.2, 0.4, 0.5, 0.5, 0.5, 0.5, 0.4, 0.2 },
            new List<double> { 0.3, 0.4, 0.5, 0.5, 0.5, 0.5, 0.4, 0.3 },
            new List<double> { 0.4, 0.4, 0.5, 0.5, 0.5, 0.5, 0.4, 0.3 },
            new List<double> { 0.2, 0.5, 0.5, 0.5, 0.5, 0.5, 0.4, 0.2 },
            new List<double> { 0.2, 0.4, 0.5, 0.4, 0.4, 0.4, 0.4, 0.2 },
            new List<double> { 0.0, 0.2, 0.2, 0.3, 0.3, 0.2, 0.2, 0.0 },
        };

        private List<List<double>> pawnScores = new List<List<double>>
        {
            new List<double> { 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8 },
            new List<double> { 0.7, 0.7, 0.7, 0.7, 0.7, 0.7, 0.7, 0.7 },
            new List<double> { 0.3, 0.3, 0.4, 0.5, 0.5, 0.4, 0.3, 0.3 },
            new List<double> { 0.25, 0.25, 0.3, 0.45, 0.45, 0.3, 0.25, 0.25 },
            new List<double> { 0.2, 0.2, 0.2, 0.4, 0.4, 0.2, 0.2, 0.2 },
            new List<double> { 0.25, 0.15, 0.1, 0.2, 0.2, 0.1, 0.15, 0.25 },
            new List<double> { 0.25, 0.3, 0.3, 0.0, 0.0, 0.3, 0.3, 0.25 },
            new List<double> { 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2 },
        };

        private Dictionary<string, List<List<double>>> piecePositionScores = new();
        private const int CHECKMATE = 1000;
        private const int STALEMATE = 0;
        private const int DEPTH = 4;
        public Move? nextMove = null;

        public MinMaxEngine()
        {
            piecePositionScores = new Dictionary<string, List<List<double>>>
            {
                { "wN", ListUtilities.Copy2dList(knightScores) },
                { "bN", ListUtilities.Copy2dList(knightScores, true) },
                { "wB", ListUtilities.Copy2dList(bishopScores) },
                { "bB", ListUtilities.Copy2dList(bishopScores, true) },
                { "wQ", ListUtilities.Copy2dList(queenScores) },
                { "bQ", ListUtilities.Copy2dList(queenScores, true) },
                { "wR", ListUtilities.Copy2dList(rookScores) },
                { "bR", ListUtilities.Copy2dList(rookScores, true) },
                { "wP", ListUtilities.Copy2dList(pawnScores) },
                { "bP", ListUtilities.Copy2dList(pawnScores, true) }
            };
        }

        public void FindBestMove(Game game, List<Move> validMoves)
        {
            nextMove = null;
            ListUtilities.Shuffle(validMoves);
            FindMoveNegaMaxAlphaBeta(game, validMoves, DEPTH, -CHECKMATE, CHECKMATE, game.state.whiteToMove ? 1 : -1);
        }

        public void FindRandomMove(Game game, List<Move> validMoves)
        {
            ListUtilities.Shuffle(validMoves);
            nextMove = validMoves[0];
        }

        private Double FindMoveNegaMaxAlphaBeta(Game game, List<Move> validMoves, int depth, double alpha, double beta, int turnMultiplier)
        {
            if (depth == 0)
            {
                return turnMultiplier * ScoreBoard(game);
            }
            Double maxScore = -CHECKMATE;
            foreach (Move m in validMoves)
            {
                GameHandler.MakeMove(game, m);
                var nextMoves = GameHandler.FindValidMoves(game);
                double score = -FindMoveNegaMaxAlphaBeta(game, nextMoves, depth - 1, -beta, -alpha, -turnMultiplier);
                if (score > maxScore)
                {
                    maxScore = score;
                    if (depth == DEPTH)
                    {
                        nextMove = m;
                    }
                }
                GameHandler.UndoMove(game);
                if (maxScore > alpha)
                {
                    alpha = maxScore;
                }
                if (alpha >= beta)
                {
                    break;
                }
            }
            return maxScore;
        }

        private Double ScoreBoard(Game game)
        {
            if (game.state.checkMate)
            {
                if (game.state.whiteToMove)
                {
                    return -CHECKMATE;
                }
                else
                {
                    return CHECKMATE;
                }
            }
            else if (game.state.staleMate)
            {
                return STALEMATE;
            }
            double score = 0;
            for (int row = 0; row < game.state.board.Count; row++)
            {
                for (int col = 0; col < game.state.board[row].Count; col++)
                {
                    string piece = game.state.board[row][col];
                    if (piece != "--")
                    {
                        double piecePositionScore = 0;
                        if (piece[1].ToString() != "K")
                        {
                            piecePositionScore = piecePositionScores[piece][row][col];
                        }
                        if (piece[0].ToString() == "w")
                        {
                            score += pieceScores[piece[1].ToString()] + piecePositionScore;
                        }
                        if (piece[0].ToString() == "b")
                        {
                            score -= pieceScores[piece[1].ToString()] + piecePositionScore;
                        }
                    }
                }
            }
            return score;
        }
    }
}






