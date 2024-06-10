using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GameState
    {
        Position _position = new Position();
        public Dictionary<char, Func<int[], List<Move>>> moveFunctions =
            new Dictionary<char, Func<int[], List<Move>>>();
        public List<Move> moveLog = new List<Move>();
        public bool inCheck = false;
        List<Pin> pins = new List<Pin>();
        List<Check> checks = new List<Check>();
        public bool isCheckMate = false;
        public bool isStaleMate = false;

        public GameState()
        {
            /*moveFunctions.Add('P', getPawnMoves);
            moveFunctions.Add('R', getRookMoves);
            moveFunctions.Add('N', getKnightMoves);
            moveFunctions.Add('B', getBishopMoves);
            moveFunctions.Add('Q', getQueenMoves);
            moveFunctions.Add('K', getKingMoves);*/
        }
    }
}
