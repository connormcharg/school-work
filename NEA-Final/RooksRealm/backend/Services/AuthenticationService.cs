namespace backend.Services
{
    using backend.Classes.Data;
    using backend.Classes.Utilities;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="AuthenticationService" />
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Defines the userRepository
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Defines the jwtSecret
        /// </summary>
        private readonly string jwtSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="userRepository">The userRepository<see cref="IUserRepository"/></param>
        /// <param name="jwtSecret">The jwtSecret<see cref="string"/></param>
        public AuthenticationService(IUserRepository userRepository, string jwtSecret)
        {
            this.userRepository = userRepository;
            this.jwtSecret = jwtSecret;
        }

        /// <summary>
        /// The Authenticate
        /// </summary>
        /// <param name="email">The email<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string Authenticate(string email, string password)
        {
            var user = userRepository.GetUserByEmail(email);

            if (user == null || !VerifyPasswordHash(password, user.storedHashValue))
            {
                return "";
            }

            return GenerateJwtToken(user);
        }

        /// <summary>
        /// The VerifyPasswordHash
        /// </summary>
        /// <param name="password">The password<see cref="string"/></param>
        /// <param name="storedvalue">The storedvalue<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool VerifyPasswordHash(string password, string storedvalue)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(storedvalue))
            {
                return false;
            }
            return SecurityUtilities.VerifyPassword(password, storedvalue);
        }

        /// <summary>
        /// The GenerateJwtToken
        /// </summary>
        /// <param name="user">The user<see cref="User"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(ClaimTypes.Name, user.username)
                    // additional claims can go here
                }),
                /*Expires = DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),*/
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
