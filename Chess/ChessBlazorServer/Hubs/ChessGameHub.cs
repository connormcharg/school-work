using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChessEngine;

namespace ChessBlazorServer.Hubs
{
    public class ChessGameHub : Hub
    {
        GameState gameState = new GameState();

        public async Task SendMove(int startRow, int startCol, int endRow, int endCol)
        {
            // Make Move
            var validMoves = gameState.getValidMoves();
            var move = new Move(new List<int> { startRow, startCol }, new List<int> { endRow, endCol }, gameState.board);

            for (int i = 0; i < validMoves.Count; i++)
            {
                if (move.Equals(validMoves[i]))
                {
                    gameState.makeMove(validMoves[i]);
                    break;
                }
            }

            await Clients.All.SendAsync("ReceiveMove", gameState.board);
        }

        public async Task GetBoardState()
        {
            await Clients.All.SendAsync("ReceiveBoardState", gameState.board);
        }
    }
}