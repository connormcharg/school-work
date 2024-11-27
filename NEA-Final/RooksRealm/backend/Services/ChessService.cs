namespace backend.Services
{
    using backend.Classes.Handlers;
    using backend.Classes.State;
    using backend.Hubs;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ChessService" />
    /// </summary>
    public class ChessService
    {
        /// <summary>
        /// Defines the games
        /// </summary>
        private Dictionary<string, Game> games = new Dictionary<string, Game>();

        /// <summary>
        /// Defines the hubContext
        /// </summary>
        private readonly IHubContext<ChessHub> hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChessService"/> class.
        /// </summary>
        /// <param name="hubContext">The hubContext<see cref="IHubContext{ChessHub}"/></param>
        public ChessService(IHubContext<ChessHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        /// <summary>
        /// The GetGame
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Game?"/></returns>
        public Game? GetGame(string id)
        {
            if (games.TryGetValue(id, out var game))
            {
                return new Game(game);
            }
            return null;
        }

        /// <summary>
        /// The GetAllGames
        /// </summary>
        /// <returns>The <see cref="List{Game}"/></returns>
        public List<Game> GetAllGames()
        {
            return games.Values.Select(g => new Game(g)).ToList();
        }

        /// <summary>
        /// The AddGame
        /// </summary>
        /// <param name="game">The game<see cref="Game"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool AddGame(Game game)
        {
            return games.TryAdd(game.id, new Game(game));
        }

        /// <summary>
        /// The RemoveGame
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool RemoveGame(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                return false;
            }
            return games.Remove(id);
        }

        /// <summary>
        /// The ArchiveGame
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool ArchiveGame(string id)
        {
            return false;
        }

        // Better update functions

        /// <summary>
        /// The UpdateClients
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task UpdateClients(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var gameCopy = new Game(game);
            var json = JsonConvert.SerializeObject(gameCopy);
            await hubContext.Clients.Group(id).SendAsync("ReceiveGame", json);
        }

        /// <summary>
        /// The StartTimer
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="isWhiteTimer">The isWhiteTimer<see cref="bool"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task StartTimer(string id, bool isWhiteTimer)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (isWhiteTimer)
            {
                game.state.whiteTimeRunning = true;
            }
            else
            {
                game.state.blackTimeRunning = true;
            }
            await UpdateClients(id);
        }

        /// <summary>
        /// The StopTimers
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task StopTimers(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.state.whiteTimeRunning = false;
            game.state.blackTimeRunning = false;
            await UpdateClients(id);
        }

        /// <summary>
        /// The UpdateTimers
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task UpdateTimers(string id)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.whiteTimeRunning)
            {
                game.state.whiteTime -= 1;
            }
            if (game.state.blackTimeRunning)
            {
                game.state.blackTime -= 1;
            }
            if (game.state.whiteTime <= 0 || game.state.blackTime <= 0)
            {
                game.state.gameOver = true;
            }
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        /// <summary>
        /// The EngineUpdate
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="moveId">The moveId<see cref="int"/></param>
        /// <param name="suggestedMoveId">The suggestedMoveId<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task EngineUpdate(string id, int moveId, int suggestedMoveId)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                return;
            }
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            var move = game.currentValidMoves.FirstOrDefault(m => m.moveID == moveId);
            if (move != null)
            {
                GameHandler.MakeMove(game, move);
                game.suggestedMoveId = suggestedMoveId;
                game.currentValidMoves = GameHandler.FindValidMoves(game);
                GameHandler.CheckGameOver(game);
                await UpdateClients(id);
            }
        }

        /// <summary>
        /// The SuggestedMoveUpdate
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="suggestedMoveId">The suggestedMoveId<see cref="int"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task SuggestedMoveUpdate(string id, int suggestedMoveId)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                return;
            }
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            game.suggestedMoveId = suggestedMoveId;
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        /// <summary>
        /// The AddPlayer
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="player">The player<see cref="Player"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task AddPlayer(string id, Player player)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.players.Add(player);
            await UpdateClients(id);
        }

        /// <summary>
        /// The JoinPlayer
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="nickname">The nickname<see cref="string"/></param>
        /// <param name="connectionId">The connectionId<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task JoinPlayer(string id, string nickname, string connectionId)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var p = game.players.FirstOrDefault(p => p.nickName == nickname);
            if (p != null)
            {
                p.connectionId = connectionId;
                p.isConnected = true;
                await UpdateClients(id);
            }
        }

        /// <summary>
        /// The RemovePlayer
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="player">The player<see cref="Player"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RemovePlayer(string id, Player player)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var p = game.players.FirstOrDefault(p => p.nickName == player.nickName);
            if (p != null)
            {
                p.isConnected = false;
                await UpdateClients(id);
            }
        }

        /// <summary>
        /// The AddWatcher
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="watcher">The watcher<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task AddWatcher(string id, string watcher)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.watchers.Add(watcher);
            await UpdateClients(id);
        }

        /// <summary>
        /// The RemoveWatcher
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="watcher">The watcher<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task RemoveWatcher(string id, string watcher)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.watchers.Remove(watcher);
            await UpdateClients(id);
        }

        /// <summary>
        /// The PushMove
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="move">The move<see cref="Move"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task PushMove(string id, Move move)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.pauseAgreed || game.state.gameOver)
            {
                return;
            }
            GameHandler.MakeMove(game, move);
            game.currentValidMoves = GameHandler.FindValidMoves(game);
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        /// <summary>
        /// The PushResign
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="isWhite">The isWhite<see cref="bool"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task PushResign(string id, bool isWhite)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            game.state.isWhiteResignation = isWhite;
            game.state.playerResigned = true;
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        /// <summary>
        /// The PushPauseRequest
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="nickname">The nickname<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task PushPauseRequest(string id, string nickname)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.pauseRequests.Contains(nickname))
            {
                game.state.pauseRequests.Remove(nickname);
            }
            else
            {
                game.state.pauseRequests.Add(nickname);
                if ((game.state.pauseRequests.Count >= 2 && !game.settings.isSinglePlayer)
                    || (game.state.pauseRequests.Count >= 1 && game.settings.isSinglePlayer))
                {
                    game.state.pauseRequests = new List<string>();
                    game.state.pauseAgreed = !game.state.pauseAgreed;
                }
            }
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        /// <summary>
        /// The PushDrawOffer
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="nickname">The nickname<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task PushDrawOffer(string id, string nickname)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            if (game.state.drawOffers.Contains(nickname))
            {
                game.state.drawOffers.Remove(nickname);
            }
            else
            {
                game.state.drawOffers.Add(nickname);
                if (game.state.drawOffers.Count >= 2 && !game.settings.isSinglePlayer)
                {
                    game.state.drawOffers = new List<string>();
                    game.state.drawAgreed = !game.state.drawAgreed;
                }
            }
            GameHandler.CheckGameOver(game);
            await UpdateClients(id);
        }

        /// <summary>
        /// The GameOver
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="result">The result<see cref="string"/></param>
        /// <param name="reason">The reason<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task GameOver(string id, string result, string reason)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var data = new { result = result, reason = reason };
            var json = JsonConvert.SerializeObject(data);
            await hubContext.Clients.Group(id).SendAsync("ReceiveGameOver", json);
        }

        /// <summary>
        /// The GameOver
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="result">The result<see cref="string"/></param>
        /// <param name="reason">The reason<see cref="string"/></param>
        /// <param name="ratingChange">The ratingChange<see cref="string"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task GameOver(string id, string result, string reason, string ratingChange)
        {
            games.TryGetValue(id, out var game);
            if (game == null)
            {
                throw new Exception("game id not found in games dictionary");
            }
            var data = new { result = result, reason = reason, ratingChange = ratingChange };
            var json = JsonConvert.SerializeObject(data);
            await hubContext.Clients.Group(id).SendAsync("ReceiveGameOver", json);
        }
    }
}
