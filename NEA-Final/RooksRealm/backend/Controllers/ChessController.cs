namespace backend.Controllers
{
    using backend.Classes.State;
    using backend.Classes.Utilities;
    using backend.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")] // "api/chess"
    public class ChessController : ControllerBase
    {
        private readonly ChessService chessService;
        private readonly UserService userService;

        public ChessController(ChessService chessService, UserService userService)
        {
            this.chessService = chessService;
            this.userService = userService;
        }

        [HttpGet("nickname")]
        public IActionResult GetNickname()
        {
            string nickname = GeneratorUtilities.GenerateNickname();
            return Ok(nickname);
        }

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
    }
}