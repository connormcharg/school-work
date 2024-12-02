namespace backend.Controllers
{
    using backend.Classes.State;
    using backend.Classes.Utilities;
    using backend.Services;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="ChessController" />
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // "api/chess"
    public class ChessController : ControllerBase
    {
        /// <summary>
        /// Defines the chessService
        /// </summary>
        private readonly ChessService chessService;

        /// <summary>
        /// Defines the userService
        /// </summary>
        private readonly UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChessController"/> class.
        /// </summary>
        /// <param name="chessService">The chessService<see cref="ChessService"/></param>
        /// <param name="userService">The userService<see cref="UserService"/></param>
        public ChessController(ChessService chessService, UserService userService)
        {
            this.chessService = chessService;
            this.userService = userService;
        }

        /// <summary>
        /// The GetNickname
        /// </summary>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpGet("nickname")]
        public IActionResult GetNickname()
        {
            string nickname = GeneratorUtilities.GenerateNickname();
            return Ok(nickname);
        }

        /// <summary>
        /// The GetPublicGames
        /// </summary>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpGet("public")]
        public IActionResult GetPublicGameDetails()
        {
            var games = chessService.GetAllGames();
            var publicGames = games.Where((g) => !g.settings.isPrivate && g.players.Count < 2 && !g.settings.isSinglePlayer);

            var details = new List<object>();

            foreach (var game in publicGames)
            {
                details.Add(new
                {
                    game.id,
                    title = game.settings.gameTitle,
                    playerCount = game.players.Count,
                    players = game.players.Select(p => p.nickName).ToList(),
                    game.settings
                });
            }

            return Ok(details);
        }

        /// <summary>
        /// The GetPrivateGameDetails
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpGet("details")]
        public IActionResult GetPrivateGameDetails([FromQuery] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Game ID is required.");
            }

            var game = chessService.GetAllGames().FirstOrDefault(g => g.id == id);

            if (game == null)
            {
                return NotFound("Game not found.");
            }

            var details = new
            {
                game.id,
                title = game.settings.gameTitle,
                playerCount = game.players.Count,
                players = game.players.Select(p => p.nickName).ToList(),
                game.settings
            };

            return Ok(details);
        }

        /// <summary>
        /// The GetPlayerInfo
        /// </summary>
        /// <param name="connectionId">The connectionId<see cref="string"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpGet("player/{connectionId}")]
        public IActionResult GetPlayerInfo(string connectionId)
        {
            string? nickname = userService.GetNickname(connectionId);
            if (nickname == null)
            {
                return NotFound();
            }
            return Ok(nickname);
        }

        /// <summary>
        /// The StartGame
        /// </summary>
        /// <param name="settings">The settings<see cref="Settings"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpPost("start")]
        public IActionResult StartGame([FromBody] Settings settings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (settings == null)
            {
                return BadRequest();
            }

            var allGames = chessService.GetAllGames();
            var newId = GeneratorUtilities.GetNewId();

            while (allGames.Any(g => g.id == newId))
            {
                newId = GeneratorUtilities.GetNewId();
            }

            var game = new Game(settings) { id = newId };

            bool result = chessService.AddGame(game);

            if (result)
            {
                return CreatedAtAction(nameof(StartGame), new { id = newId });
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// The JoinGame
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpGet("join")]
        public IActionResult JoinGame([FromQuery] string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var games = chessService.GetAllGames();
            var game = games.FirstOrDefault(g => g.id == id);

            if (game == null)
            {
                return NotFound();
            }

            if (game.players.Count < 2 && !game.settings.isSinglePlayer)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
