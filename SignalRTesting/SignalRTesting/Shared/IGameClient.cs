using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTesting.Shared
{
    public interface IGameClient
    {
        Task RenderBoard(string[][] board);
        Task Colour(string colour);
        Task Turn(string player);
        Task RollCall(Player player1, Player player2);
        Task Concede(bool x);
        Task Victory(string player, string[][] board);
    }
}
