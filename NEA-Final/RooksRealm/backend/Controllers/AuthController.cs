namespace backend.Controllers
{
    using backend.Classes.Data;
    using backend.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

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
        [HttpPost("changeUsername")]
        public IActionResult ChangeUsername([FromBody] UsernameUpdateRequest request)
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

            var otherUser = userRepository.GetUserByUsername(request.newUsername);
            if (otherUser != null)
            {
                if (otherUser.email != user.email)
                {
                    return BadRequest("User already exists with that username");
                }
            }

            bool res = userRepository.UpdateUserNickname(user.email, request.newUsername);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("changeEmail")]
        public IActionResult ChangeEmail([FromBody] EmailUpdateRequest request)
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

            bool res = userRepository.UpdateUserEmail(user.email, request.newEmail);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("changePassword")]
        public IActionResult ChangePassword([FromBody] PasswordUpdateRequest request)
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

            bool res = userRepository.UpdateUserPassword(user.email, user.storedHashValue, request.oldPassword, request.newPassword);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("changeTheme")]
        public IActionResult UpdateTheme([FromBody] ThemeUpdateRequest request)
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

            bool res = userRepository.UpdateUserBoardTheme(user.email, request.newTheme);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpPost("delete")]
        public IActionResult DeleteAccount()
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

            bool res = userRepository.DeleteUser(user.username, user.email);
            if (res)
            {
                return Ok();
            }
            return BadRequest();
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
                email = user.email,
                boardTheme = user.boardTheme,
                rating = user.rating,
                role = user.role,
            });
        }
    }

    public class AuthRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class EmailUpdateRequest
    {
        public string newEmail { get; set; }
    }

    public class UsernameUpdateRequest
    {
        public string newUsername { get; set; }
    }

    public class PasswordUpdateRequest
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }

    public class ThemeUpdateRequest
    {
        public string newTheme { get; set; }
    }
}