using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;

namespace SignalRTesting.Shared
{
    public class ChessGame
    {
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public Player CurrentPlayer { get; set; }
        public bool InProgress { get; set; }
        public string Id { get; set; }
        public GameState gs { get; private set; }

        public ChessGame()
        {
            gs = new GameState();
            Player1 = new Player();
            Player2 = new Player();
        }

        public Player GetPlayer(string connectionId)
        {
            if (Player1 != null && Player1.ConnectionId == connectionId) return Player1;
            if (Player2 != null && Player2.ConnectionId == connectionId) return Player2;
            return null;
        }

        public bool HasPlayer(string connectionId)
        {
            if (Player1 != null && Player1.ConnectionId == connectionId) return true;
            if (Player2 != null && Player2.ConnectionId == connectionId) return true;
            return false;
        }

        public void NextPlayer()
        {
            if (CurrentPlayer == Player1) CurrentPlayer = Player2;
            else CurrentPlayer = Player1;
        }

        public string CheckEnding()
        {
            if (gs.staleMate) return "stalemate";
            if (gs.checkMate) return "checkmate";
            else return "";
        }

        public bool TryMakeMove(int sr, int sc, int er, int ec)
        {
            Move temp = new Move(new List<int> { sr, sc }, new List<int> { er, ec }, gs.board);
            var validMoves = gs.getValidMoves();
            var valid = false;
            foreach (var move in validMoves)
            {
                if (move.Equals(temp))
                {
                    valid = true;
                    gs.makeMove(move);
                }
            }
            return valid;
        }
    }
}
