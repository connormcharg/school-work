using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTesting.Shared
{
    public interface IChessGameRepository
    {
        List<ChessGame> Games { get; }
    }

    public class ChessGameRepository : IChessGameRepository
    {
        public ChessGameRepository() { }

        public List<ChessGame> Games { get; } = new List<ChessGame>();
    }
}
