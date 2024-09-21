using CheckAndMate.Hubs;
using CheckAndMate.Services;
using CheckAndMate.Shared.Chess;
using CheckAndMate.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Formats.Asn1;

namespace CheckAndMate.Controllers
{
    [ApiController]
    [Route("api/chess")]
    public class ChessController : ControllerBase
    {
        private readonly ChessService _chessService;
        private readonly UserService _userService;

        public ChessController(ChessService chessService, UserService userService)
        {
            _chessService = chessService;
            _userService = userService;
        }

        // GET: api/chess/public
        [HttpGet("public")]
        public IActionResult GetPublicGames()
        {
            var games = _chessService.GetAllGames();
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

        // GET: api/chess/{id}
        [HttpGet("{id}")]
        public IActionResult GetGameDetails(string id)
        {
            var game = _chessService.GetAllGames().FirstOrDefault(g => g.id == id);
            if (game == null)
                return NotFound();
            return Ok(game);
        }

        // GET: api/chess/player/{connectionId}
        [HttpGet("player/{connectionId}")]
        public async Task<IActionResult> GetPlayerInfo(string connectionId)
        {
            string nickname = await _userService.GetNicknameAsync(connectionId);
            var player = new List<string>();
            player.Add(nickname);

            return Ok(player);
        }

        // POST: api/chess/start
        [HttpPost("start")]
        public IActionResult StartGame([FromBody] GameSettings gameSettings)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (gameSettings == null)
            {
                return BadRequest();
            }

            var allGames = _chessService.GetAllGames();
            var newId = Util.GetNewId();

            while (allGames.Any(g => g.id == newId))
            {
                newId = Util.GetNewId();
            }

            var game = new Game(gameSettings) { id = newId };

            bool result = _chessService.AddGame(game);

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
