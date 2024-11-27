namespace backend.Controllers
{
    using backend.Classes.Data;
    using backend.Services;
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
        /// Defines the userService
        /// </summary>
        private readonly UserService userService;

        /// <summary>
        /// Defines the authenticationService
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticController"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository<see cref="IUserRepository"/></param>
        /// <param name="statisticsRepository">The statisticsRepository<see cref="IStatisticsRepository"/></param>
        /// <param name="userService">The userService<see cref="UserService"/></param>
        /// <param name="authenticationService">The authenticationService<see cref="IAuthenticationService"/></param>
        public StatisticController(IUserRepository userRepository, IStatisticsRepository statisticsRepository, UserService userService, IAuthenticationService authenticationService)
        {
            this.userRepository = userRepository;
            this.statisticsRepository = statisticsRepository;
            this.userService = userService;
            this.authenticationService = authenticationService;
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
