using backend.Classes.State;
using backend.Classes.Utilities;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.Controllers
{
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

        [HttpGet("public")]
        public IActionResult GetPublicGames()
        {
            var games = chessService.GetAllGames();
            var publicGames = new List<Game>();

            foreach (Game g in games)
            {
                if (!g.settings.isPrivate)
                {
                    publicGames.Add(g);
                }
            }

            return Ok(publicGames);
        }

        [HttpGet("{id}")]
        public IActionResult GetGameDetails(string id)
        {
            var game = chessService.GetGame(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
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
