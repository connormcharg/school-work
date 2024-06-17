using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public interface IPiece
    {
        public bool isWhite { get; }
        
        public string ToString(); // wp, bq ect...
    }
}
