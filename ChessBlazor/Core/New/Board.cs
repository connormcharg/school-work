using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public class Board
    {
        public List<List<IPiece>> _board;

        public Board(string[][] layout, Dictionary<char, string> charsToTypes)
        {
            _board = new();
            FillBoard(layout, charsToTypes);
        }

        private void FillBoard(string[][] layout, Dictionary<char, string> charsToTypes)
        {
            for (int i = 0; i < layout.Length; i++)
            {
                _board.Add(new List<IPiece>());
                for (int j = 0; j < layout[i].Length; j++)
                {
                    _board[i].Add(CreateObject(charsToTypes[layout[i][j][1]], layout[i][j][0] == 'w'));
                }
            }
        }

        private IPiece CreateObject(string typeName, bool isWhite)
        {
            Type? t = Type.GetType($"Core.New.{typeName}");

            if (t != null && typeof(IPiece).IsAssignableFrom(t) && t != typeof(IPiece))
            {
                object? instance;
                if (typeName == "Empty")
                {
                    instance = Activator.CreateInstance(t);
                }
                else
                {
                    instance = Activator.CreateInstance(t, args: isWhite);
                }
                return (IPiece)instance;
            }
            else
            {
                throw new ArgumentException("Invalid type name or type does not inherit from IPiece");
            }
        }

        public IPiece this[(int row, int col) position]
        {
            get
            {
                return _board[position.row][position.col];
            }
            set
            {
                _board[position.row][position.col] = value;
            }
        }
    }
}
