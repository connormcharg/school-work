using Microsoft.AspNetCore.Mvc;
using SignalRTesting.Shared;

namespace SignalRTesting.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        private readonly IGameRepository _repository;

        public LeaderBoardController(IGameRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<HighScore> Get()
        {
            var scores = _repository.HighScores.OrderByDescending(s => s.Percentage).Take(10);

            return scores.ToArray();
        }
    }
}
