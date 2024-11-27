namespace backend.Controllers
{
    using backend.Classes.Data;
    using backend.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Security.Claims;

    /// <summary>
    /// Defines the <see cref="AuthController" />
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] // "api/auth"
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Defines the authenticationService
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// Defines the userRepository
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Defines the userService
        /// </summary>
        private readonly UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService<see cref="IAuthenticationService"/></param>
        /// <param name="userRepository">The userRepository<see cref="IUserRepository"/></param>
        /// <param name="userService">The userService<see cref="UserService"/></param>
        public AuthController(IAuthenticationService authenticationService, IUserRepository userRepository,
            UserService userService)
        {
            this.authenticationService = authenticationService;
            this.userRepository = userRepository;
            this.userService = userService;
        }

        /// <summary>
        /// The Register
        /// </summary>
        /// <param name="request">The request<see cref="AuthRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The Login
        /// </summary>
        /// <param name="request">The request<see cref="AuthRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The ChangeUsername
        /// </summary>
        /// <param name="request">The request<see cref="UsernameUpdateRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The ChangeEmail
        /// </summary>
        /// <param name="request">The request<see cref="EmailUpdateRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The ChangePassword
        /// </summary>
        /// <param name="request">The request<see cref="PasswordUpdateRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The UpdateTheme
        /// </summary>
        /// <param name="request">The request<see cref="ThemeUpdateRequest"/></param>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The DeleteAccount
        /// </summary>
        /// <returns>The <see cref="IActionResult"/></returns>
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

        /// <summary>
        /// The GetDetails
        /// </summary>
        /// <returns>The <see cref="IActionResult"/></returns>
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

    /// <summary>
    /// Defines the <see cref="AuthRequest" />
    /// </summary>
    public class AuthRequest
    {
        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string password { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="EmailUpdateRequest" />
    /// </summary>
    public class EmailUpdateRequest
    {
        /// <summary>
        /// Gets or sets the newEmail
        /// </summary>
        public string newEmail { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="UsernameUpdateRequest" />
    /// </summary>
    public class UsernameUpdateRequest
    {
        /// <summary>
        /// Gets or sets the newUsername
        /// </summary>
        public string newUsername { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PasswordUpdateRequest" />
    /// </summary>
    public class PasswordUpdateRequest
    {
        /// <summary>
        /// Gets or sets the oldPassword
        /// </summary>
        public string oldPassword { get; set; }

        /// <summary>
        /// Gets or sets the newPassword
        /// </summary>
        public string newPassword { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="ThemeUpdateRequest" />
    /// </summary>
    public class ThemeUpdateRequest
    {
        /// <summary>
        /// Gets or sets the newTheme
        /// </summary>
        public string newTheme { get; set; }
    }
}
