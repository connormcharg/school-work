using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckAndMate.Shared.Utilities;

namespace CheckAndMate.Shared.Chess
{
    public class ChessEngine
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

        public ChessEngine()
        {
            piecePositionScores = new Dictionary<string, List<List<double>>>
            {
                { "wN", Util.Copy2dList(knightScores) },
                { "bN", Util.Copy2dList(knightScores, true) },
                { "wB", Util.Copy2dList(bishopScores) },
                { "bB", Util.Copy2dList(bishopScores, true) },
                { "wQ", Util.Copy2dList(queenScores) },
                { "bQ", Util.Copy2dList(queenScores, true) },
                { "wR", Util.Copy2dList(rookScores) },
                { "bR", Util.Copy2dList(rookScores, true) },
                { "wP", Util.Copy2dList(pawnScores) },
                { "bP", Util.Copy2dList(pawnScores, true) }
            };
        }


    }
}
