namespace backend.Controllers
{
    using backend.Classes.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    /// <summary>
    /// Defines the <see cref="StatisticController" />
    /// </summary>
    [ApiController]
    [Route("api/stats")] // "api/stats"
    public class StatisticController : ControllerBase
    {
        /// <summary>
        /// Defines the statisticsRepository
        /// </summary>
        private readonly IStatisticsRepository statisticsRepository;

        /// <summary>
        /// Defines the userRepository
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticController"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository<see cref="IUserRepository"/></param>
        /// <param name="statisticsRepository">The statisticsRepository<see cref="IStatisticsRepository"/></param>
        public StatisticController(IUserRepository userRepository, IStatisticsRepository statisticsRepository)
        {
            this.userRepository = userRepository;
            this.statisticsRepository = statisticsRepository;
        }

        /// <summary>
        /// The GetMessages
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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
