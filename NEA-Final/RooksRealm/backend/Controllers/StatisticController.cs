namespace backend.Controllers
{
    using backend.Classes.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [ApiController]
    [Route("api/stats")] // "api/stats"
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticsRepository statisticsRepository;
        private readonly IUserRepository userRepository;

        public StatisticController(IUserRepository userRepository, IStatisticsRepository statisticsRepository)
        {
            this.userRepository = userRepository;
            this.statisticsRepository = statisticsRepository;
        }

        [Authorize]
        [HttpGet("")]
        public IActionResult GetStatistics([FromQuery] int daysAgo)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var user = userRepository.GetUserById(int.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }

            var statistics = statisticsRepository.GetStatistics(daysAgo, user.id);
            if (statistics == null)
            {
                statistics = new List<Statistic>();
            }

            return Ok(new { statistics = statistics });
        }
    }
}