using backend.Classes.Data;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // "api/auth"
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IUserRepository userRepository;
        private readonly UserService userService;

        public AuthController(IAuthenticationService authenticationService, IUserRepository userRepository,
            UserService userService)
        {
            this.authenticationService = authenticationService;
            this.userRepository = userRepository;
            this.userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthRequest request)
        {
            var possibleUser = userRepository.GetUserByEmail(request.email);
            if (possibleUser != null)
            {
                return BadRequest(new { message = "User already registered with that email" });
            }
            
            var result = userRepository.CreateUser(userService.GenerateUniqueNickname(), request.email, request.password);

            if (!result)
            {
                return BadRequest(new { message = "User registration failed" });
            }

            return Ok(new { message = "User registered successfully" });
        }

        // Endpoint for user login
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            var token = authenticationService.Authenticate(request.email, request.password);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok("This is protected data");
        }

        /*
        
        changeEmail = (token, new) => ()
        changeUsername = (token, new) => ()
        changePassword = (token, old, new) => ()
        deleteAccount = (token, password) => ()

        */

        [Authorize]
        [HttpPost("changeEmail")]
        public IActionResult ChangeEmail()
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


        }

        [Authorize]
        [HttpGet("details")]
        public IActionResult GetDetails()
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

            return Ok(new
            {
                username = user.username,
                email = user.email
            });
        }
    }

    public class AuthRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class DetailsRequest
    {
        public string token { get; set; }
    }
}
