using backend.Classes.Data;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using backend.Classes.Utilities;

namespace backend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository userRepository;
        private readonly string jwtSecret;
        private readonly int jwtExpirationMinutes;

        public AuthenticationService(IUserRepository userRepository, string jwtSecret, int jwtExpirationMinutes)
        {
            this.userRepository = userRepository;
            this.jwtSecret = jwtSecret;
            this.jwtExpirationMinutes = jwtExpirationMinutes;
        }

        public string Authenticate(string username, string password)
        {
            var user = userRepository.GetUserByUsername(username);

            if (user == null || !VerifyPasswordHash(password, user.storedHashValue))
            {
                return "";
            }

            return GenerateJwtToken(user);
        }

        private bool VerifyPasswordHash(string password, string storedvalue)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(storedvalue))
            {
                return false;
            }
            return SecurityUtilities.VerifyPassword(password, storedvalue);
        }

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
                Expires = DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
