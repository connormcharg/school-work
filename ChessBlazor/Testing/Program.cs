using Core;
using Core.New;

namespace ConsoleTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ClassicGameState g = new();

            DisplayGame(g);

            Move m = new Move((6, 4), (4, 4), g.board, false, false);
            Console.WriteLine(m.pieceCaptured);
        }

        static void DisplayGame(IGameState g)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(g.board[(i, j)].ToString() + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
