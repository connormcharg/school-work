using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.New
{
    public interface IGameState
    {
        public List<List<IPiece>> board { get; set; }
    }
}
