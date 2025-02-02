﻿namespace backend.Services
{
    using backend.Classes.Data;
    using backend.Classes.Utilities;
    using System.Security.Claims;

    /// <summary>
    /// Defines the <see cref="UserService" />
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Defines the connectionMappingService
        /// </summary>
        private readonly ConnectionMappingService connectionMappingService;

        /// <summary>
        /// Defines the userRepository
        /// </summary>
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="connectionMappingService">The connectionMappingService<see cref="ConnectionMappingService"/></param>
        /// <param name="userRepository">The userRepository<see cref="IUserRepository"/></param>
        public UserService(ConnectionMappingService connectionMappingService,
            IUserRepository userRepository)
        {
            this.connectionMappingService = connectionMappingService;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// The IsNicknameTaken
        /// </summary>
        /// <param name="nickname">The nickname<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool IsNicknameTaken(string nickname)
        {
            var userWithNickname = userRepository.GetUserByUsername(nickname);
            return userWithNickname != null;
        }

        /// <summary>
        /// The GenerateUniqueNickname
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
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

        /// <summary>
        /// The GetRating
        /// </summary>
        /// <param name="connectionId">The connectionId<see cref="string"/></param>
        /// <returns>The <see cref="int"/></returns>
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

        /// <summary>
        /// The GetNickname
        /// </summary>
        /// <param name="connectionId">The connectionId<see cref="string"/></param>
        /// <returns>The <see cref="string?"/></returns>
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
