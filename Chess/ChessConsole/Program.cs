using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace ChessConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameState gs = new GameState();
            var validMoves = gs.getValidMoves();

            var running = true;

            while (running)
            {
                Console.Clear();
                displayBoard(gs);
                var white = "white";
                var black = "black";
                var start = new List<int>();
                var end = new List<int>();

                Console.WriteLine(validMoves.Count);

                Console.Write($"Enter a start row ({(gs.whiteToMove ? white : black)}): ");
                start.Add(int.Parse(Console.ReadLine()));
                Console.Write($"Enter a start col ({(gs.whiteToMove ? white : black)}): ");
                start.Add(int.Parse(Console.ReadLine()));

                Console.Write($"Enter a end row ({(gs.whiteToMove ? white : black)}): ");
                end.Add(int.Parse(Console.ReadLine()));
                Console.Write($"Enter a end col ({(gs.whiteToMove ? white : black)}): ");
                end.Add(int.Parse(Console.ReadLine()));

                var move = new Move(start, end, gs.board);
                foreach(var m in validMoves)
                {
                    if (m.Equals(move))
                    {
                        gs.makeMove(m);
                    }
                }

                Console.Clear();
                displayBoard(gs);

                validMoves = gs.getValidMoves();

                if (gs.checkMate)
                {
                    running = false;
                    if (gs.whiteToMove)
                    {
                        Console.WriteLine("Black wins by checkmate!");
                    }
                    else
                    {
                        Console.WriteLine("White wins by checkmate!");
                    }
                }
                else if (gs.staleMate)
                {
                    running = false;
                    Console.WriteLine("Stalemate!");
                }
            }

        }

        static void displayBoard(GameState gamestate)
        {
            if (gamestate == null)
            {
                return;
            }

            foreach (var row in gamestate.board)
            {
                foreach (var square in row)
                {
                    Console.Write(square + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
