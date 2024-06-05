using Microsoft.AspNetCore.SignalR;
using SignalRTesting.Shared;

namespace SignalRTesting.Server.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        private IGameRepository _repository;
        private readonly Random _random;

        public GameHub(IGameRepository repository, Random random)
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

        public async Task ColumnClick(int column)
        {
            var game = _repository.Games.FirstOrDefault(g => g.HasPlayer(Context.ConnectionId));
            if (game is null)
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
            
            var result = game.TryGetNextOpenRow(column);
            if (!result.exists)
            {
                return;
            }

            await Clients.Group(game.Id.ToString()).RenderBoard(game.Board);

            if (game.CheckVictory(result.row, column))
            {
                if (game.CurrentPlayer == game.Player1)
                {
                    UpdateHighScore(game.Player1, game.Player2);
                }
                else
                {
                    UpdateHighScore(game.Player2, game.Player1);
                }

                await Clients.Group(game.Id).Victory(game.CurrentPlayer.Colour, game.Board);
                _repository.Games.Remove(game);
                return;
            }

            game.NextPlayer();
            await Clients.Group(game.Id).Turn(game.CurrentPlayer.Colour);
        }

        public override async Task OnConnectedAsync()
        {
            var game = _repository.Games.FirstOrDefault(g => !g.InProgress);
            if (game is null)
            {
                game = new Game();
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
                await Clients.Group(game.Id.ToString()).RenderBoard(game.Board);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var game = _repository.Games.FirstOrDefault(g => g.Player1.ConnectionId == Context.ConnectionId || g.Player2.ConnectionId == Context.ConnectionId);
            if (!(game is null))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id);
                await Clients.Group(game.Id).Concede(true);
                _repository.Games.Remove(game);
            }

            await base.OnDisconnectedAsync(exception);
        }

        private void UpdateHighScore(Player winner, Player loser)
        {
            var winnerScore = _repository.HighScores.FirstOrDefault(s => s.PlayerName == winner.Name);
            if (winnerScore == null)
            {
                winnerScore = new HighScore { PlayerName = winner.Name };
                _repository.HighScores.Add(winnerScore);
            }

            winnerScore.Played++;
            winnerScore.Won++;
            winnerScore.Percentage = Convert.ToInt32((winnerScore.Won / Convert.ToSingle(winnerScore.Played)) * 100);

            var loserScore = _repository.HighScores.FirstOrDefault(s => s.PlayerName == loser.Name);
            if (loserScore == null)
            {
                loserScore = new HighScore { PlayerName = winner.Name };
                _repository.HighScores.Add(loserScore);
            }

            loserScore.Played++;
            loserScore.Percentage = Convert.ToInt32((loserScore.Won / Convert.ToSingle(loserScore.Played)) * 100);
        }

        private async void CoinToss(Game game)
        {
            var result = _random.Next(2);
            if (result == 1)
            {
                game.Player1.Colour = Game.RedCell;
                game.Player2.Colour = Game.YellowCell;
                game.CurrentPlayer = game.Player1;
            }
            else
            {
                game.Player1.Colour = Game.YellowCell;
                game.Player2.Colour = Game.RedCell;
                game.CurrentPlayer = game.Player2;
            }

            await Clients.Client(game.Player1.ConnectionId).Colour(game.Player1.Colour);
            await Clients.Client(game.Player2.ConnectionId).Colour(game.Player2.Colour);
            await Clients.Group(game.Id).Turn(Game.RedCell);
        }
    }
}
