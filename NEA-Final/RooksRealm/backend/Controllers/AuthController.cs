using backend.Classes.Data;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var result = userRepository.CreateUser(userService.GenerateUniqueNickname(), request.email, request.password);

            if (!result)
            {
                return BadRequest(new { message = "User registration failed" });
            }

            return Ok(new { message = "User registered successfully" });
        }

        // Endpoint for user login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
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
    }

    public class RegisterRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
