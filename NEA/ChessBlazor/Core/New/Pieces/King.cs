using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public class King : IPiece
    {
        public bool isWhite { get; }

        public King(bool isWhite)
        {
            this.isWhite = isWhite;
        }

        public override string ToString()
        {
            return (isWhite ? "w" : "b") + "K";
        }
    }
}
