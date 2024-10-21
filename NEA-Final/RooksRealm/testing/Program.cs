using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using backend.Classes.Engine;
using backend.Classes.State;
using backend.Classes.Handlers;

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
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Testing>();
        }
    }
}
