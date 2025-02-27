using backend.Classes.Engine;
using backend.Classes.Handlers;
using backend.Classes.State;
using BenchmarkDotNet.Attributes;

namespace testing
{
    public class Testing
    {
        [Benchmark(Description = "Engine")]
        public void EngineRun()
        {
            var e = new MinMaxEngine();
            var s = new Settings(true, true, false, false, true, true, "test");
            var g = new Game(s);
            var v = GameHandler.FindValidMoves(g);
            e.FindBestMove(g, v);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            /*BenchmarkRunner.Run<Testing>();*/
            var b = new List<List<string>>()
            {
                new List<string>() { "--", "--", "--", "--", "--", "--", "--", "bR" },
                new List<string>() { "bP", "--", "bP", "--", "--", "bK", "wN", "bP" },
                new List<string>() { "wP", "--", "--", "--", "--", "bP", "--", "wQ" },
                new List<string>() { "--", "--", "wR", "--", "--", "--", "--", "--" },
                new List<string>() { "--", "--", "--", "--", "bQ", "--", "--", "--" },
                new List<string>() { "--", "bP", "--", "--", "--", "bB", "wP", "--" },
                new List<string>() { "--", "--", "--", "--", "--", "wP", "--", "wP" },
                new List<string>() { "--", "--", "--", "--", "--", "--", "wK", "--" }
            };

            var g = new Game(new Settings(true, true, false, false, false, false, "test"));
            g.state.board = b;
            g.state.currentCastlingRight = new CastleRights(false, false, false, false);
            g.state.castleRightsLog.Add(new CastleRights(false, false, false, false));
            g.state.whiteToMove = false;
            g.state.whiteKingLocation = new List<int>() { 7, 6 };
            g.state.blackKingLocation = new List<int>() { 1, 5 };
            g.currentValidMoves = GameHandler.FindValidMoves(g);

            /*var e = new MinMaxEngine();
            e.FindBestMove(g, g.currentValidMoves);
            Console.WriteLine(e.nextMove.moveID);*/
        }
    }
}