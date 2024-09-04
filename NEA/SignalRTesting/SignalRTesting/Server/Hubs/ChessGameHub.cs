using Microsoft.AspNetCore.SignalR;
using SignalRTesting.Shared;
using System.Data;

namespace SignalRTesting.Server.Hubs
{
    public class ChessGameHub : Hub<IGameClient>
    {
        private IChessGameRepository _repository;
        private readonly Random _random;

        public ChessGameHub(IChessGameRepository repository, Random random)
        {
            _repository = repository;
            _random = random;
        }

        public async Task UpdateUser(string email, string name)
        {
            var game = _repository.Games.FirstOrDefault(g => g.HasPlayer(Context.ConnectionId));
            if (game != null)
            {
                var player = game.GetPlayer(Context.ConnectionId);
                player.Email = email;
                player.Name = name;
                await Clients.Group(game.Id).RollCall(game.Player1, game.Player2);
            }
        }

        public async Task AttemptMove(int row, int col, int row2, int col2)
        {
            var game = _repository.Games.FirstOrDefault(g => g.HasPlayer(Context.ConnectionId));
            if (game == null)
            {
                return;
            }
            if (Context.ConnectionId != game.CurrentPlayer.ConnectionId)
            {
                return;
            }
            if (!game.InProgress)
            {
                return;
            }

            var result = game.TryMakeMove(row, col, row2, col2);
            if (!result)
            {
                return;
            }

            await Clients.Group(game.Id.ToString()).RenderBoard(ConvertBoard(game.gs.board));

            var ending = game.CheckEnding();

            if (ending == "checkmate")
            {
                await Clients.Group(game.Id).Victory(game.CurrentPlayer.Colour, ConvertBoard(game.gs.board));
                _repository.Games.Remove(game);
                return;
            }
            else if (ending == "stalemate")
            {
                await Clients.Group(game.Id).Victory("Stalemate", ConvertBoard(game.gs.board));
                _repository.Games.Remove(game);
                return;
            }

            game.NextPlayer();
            await Clients.Group(game.Id).Turn(game.CurrentPlayer.Colour);
        }

        private string[][] ConvertBoard(List<List<string>> board)
        {
            var result = new string[][]
            {
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" },
                new string[] { "--", "--", "--", "--", "--", "--", "--", "--" }
            };
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board[i].Count; j++)
                {
                    result[i][j] = board[i][j];
                }
            }
            return result;
        }

        public override async Task OnConnectedAsync()
        {
            var game = _repository.Games.FirstOrDefault(g => !g.InProgress);
            if (game == null)
            {
                game = new ChessGame();
                game.Id = Guid.NewGuid().ToString();
                game.Player1.ConnectionId = Context.ConnectionId;
                _repository.Games.Add(game);
            }
            else
            {
                game.Player2.ConnectionId = Context.ConnectionId;
                game.InProgress = true;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await base.OnConnectedAsync();

            if (game.InProgress)
            {
                CoinToss(game);
                await Clients.Group(game.Id.ToString()).RenderBoard(ConvertBoard(game.gs.board));
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var game = _repository.Games.FirstOrDefault(g => g.Player1.ConnectionId == Context.ConnectionId || g.Player2.ConnectionId == Context.ConnectionId);
            if (!(game == null))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id);
                await Clients.Group(game.Id).Concede(true);
                _repository.Games.Remove(game);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        private async void CoinToss(ChessGame game)
        {
            var result = _random.Next(2);
            if (result == 1)
            {
                game.Player1.Colour = "White";
                game.Player2.Colour = "Black";
                game.CurrentPlayer = game.Player1;
            }
            else
            {
                game.Player1.Colour = "Black";
                game.Player2.Colour = "White";
                game.CurrentPlayer = game.Player2;
            }

            await Clients.Client(game.Player1.ConnectionId).Colour(game.Player1.Colour);
            await Clients.Client(game.Player2.ConnectionId).Colour(game.Player2.Colour);
            await Clients.Group(game.Id).Turn("White");
        }
    }
}
