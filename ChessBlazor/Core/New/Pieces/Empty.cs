using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public class Empty : IPiece
    {
        public bool isWhite { get; }

        public Empty()
        {
            this.isWhite = false;
        }

        public override string ToString()
        {
            return "--";
        }
    }
}
