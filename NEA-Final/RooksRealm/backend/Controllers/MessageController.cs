using backend.Classes.Data;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // "api/message "
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly UserService userService;
        private readonly IAuthenticationService authenticationService;

        public MessageController (IMessageRepository messageRepository, IUserRepository userRepository, UserService userService, IAuthenticationService authenticationService)
        {
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.userService = userService;
            this.authenticationService = authenticationService;
        }

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
    }

    public class CreateMessageRequest
    {
        public string title { get; set; }
        public string content { get; set; }
    }
}
