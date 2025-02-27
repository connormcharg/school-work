namespace backend.Services
{
    using backend.Classes.Data;
    using backend.Classes.Utilities;
    using System.Security.Claims;

    public class UserService
    {
        private readonly ConnectionMappingService connectionMappingService;
        private readonly IUserRepository userRepository;

        public UserService(ConnectionMappingService connectionMappingService,
            IUserRepository userRepository)
        {
            this.connectionMappingService = connectionMappingService;
            this.userRepository = userRepository;
        }

        public bool IsNicknameTaken(string nickname)
        {
            var userWithNickname = userRepository.GetUserByUsername(nickname);
            return userWithNickname != null;
        }

        public string GenerateUniqueNickname()
        {
            string newNickname;
            bool isTaken;

            do
            {
                newNickname = GeneratorUtilities.GenerateNickname();
                isTaken = IsNicknameTaken(newNickname);
            }
            while (isTaken);

            return newNickname;
        }

        public int GetRating(string connectionId)
        {
            var httpContext = connectionMappingService.GetHttpContext(connectionId);

            if (httpContext == null)
            {
                throw new InvalidOperationException("httpContext was null for the given connectionId");
            }

            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                // user not logged in or no valid claim
                return -1;
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID claim value");
            }

            var user = userRepository.GetUserById(userId);

            if (user == null)
            {
                // user was not found
                return -1;
            }

            var rating = user.rating;
            return rating;
        }

        public string? GetNickname(string connectionId)
        {
            var httpContext = connectionMappingService.GetHttpContext(connectionId);

            if (httpContext == null)
            {
                throw new InvalidOperationException("httpContext was null for the given connectionId");
            }

            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                // user not logged in or no valid claim
                return null;
            }

            if (!int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID claim value");
            }

            var user = userRepository.GetUserById(userId);

            if (user == null)
            {
                // user was not found
                return null;
            }

            var nickname = user.username;

            if (nickname == null)
            {
                throw new Exception("Nickname was null for a given user");
            }

            return nickname;
        }
    }
}