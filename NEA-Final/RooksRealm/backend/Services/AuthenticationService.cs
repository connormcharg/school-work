namespace backend.Services
{
    using backend.Classes.Data;
    using backend.Classes.Utilities;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository userRepository;
        private readonly string jwtSecret;

        public AuthenticationService(IUserRepository userRepository, string jwtSecret)
        {
            this.userRepository = userRepository;
            this.jwtSecret = jwtSecret;
        }

        public string Authenticate(string email, string password)
        {
            var user = userRepository.GetUserByEmail(email);

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
                }),
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}