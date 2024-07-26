using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public class Bishop : IPiece
    {
        public bool isWhite { get; }

        public Bishop(bool isWhite)
        {
            this.isWhite = isWhite;
        }

        public override string ToString()
        {
            return (isWhite ? "w" : "b") + "B";
        }
    }
}
