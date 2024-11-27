namespace backend.Controllers
{
    using backend.Classes.Data;
    using backend.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    /// <summary>
    /// Defines the <see cref="MessageController" />
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // "api/message "
    public class MessageController : ControllerBase
    {
        /// <summary>
        /// Defines the messageRepository
        /// </summary>
        private readonly IMessageRepository messageRepository;

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
        /// Initializes a new instance of the <see cref="MessageController"/> class.
        /// </summary>
        /// <param name="messageRepository">The messageRepository<see cref="IMessageRepository"/></param>
        /// <param name="userRepository">The userRepository<see cref="IUserRepository"/></param>
        /// <param name="userService">The userService<see cref="UserService"/></param>
        /// <param name="authenticationService">The authenticationService<see cref="IAuthenticationService"/></param>
        public MessageController(IMessageRepository messageRepository, IUserRepository userRepository, UserService userService, IAuthenticationService authenticationService)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.userService = userService;
            this.authenticationService = authenticationService;
        }

        /// <summary>
        /// The GetMessages
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [HttpGet("")]
        public IActionResult GetMessages([FromQuery] int daysAgo)
        {
            var messages = messageRepository.GetMessages(daysAgo);
            if (messages == null)
            {
                messages = new List<Message>();
            }

            return Ok(new { messages = messages });
        }

        /// <summary>
        /// The CreateMessage
        /// </summary>
        /// <param name="request">The request<see cref="CreateMessageRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [Authorize]
        [HttpPost("create")]
        public IActionResult CreateMessage([FromBody] CreateMessageRequest request)
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

            if (user.role == "user")
            {
                return Unauthorized("Users cannot create messages, only admins can");
            }

            bool res = messageRepository.CreateMessage(request.title, request.content, user.id);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
        }

        /// <summary>
        /// The DeleteMessage
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
        [Authorize]
        [HttpPost("delete")]
        public IActionResult DeleteMessage([FromQuery] int id)
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

            if (user.role == "user")
            {
                return Unauthorized("Users cannot delete messages, only admins can");
            }

            bool res = messageRepository.DeleteMessage(id);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
        }
    }

    /// <summary>
    /// Defines the <see cref="CreateMessageRequest" />
    /// </summary>
    public class CreateMessageRequest
    {
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string content { get; set; }
    }
}
