using CheckAndMate.Components.Account;
using CheckAndMate.Data;
using CheckAndMate.Hubs;
using CheckAndMate.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace CheckAndMate.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConnectionMappingService _connectionMappingService;

        public UserService(UserManager<ApplicationUser> userManager, ConnectionMappingService connectionMappingService)
        {
            _userManager = userManager;
            _connectionMappingService = connectionMappingService;
        }

        public bool IsNicknameTaken(string nickname)
        {
            var userWithNickname = _userManager.Users
                .FirstOrDefault(u => u.Nickname == nickname);

            return userWithNickname != null;
        }

        public string GenerateUniqueNicknameAsync()
        {
            string newNickname;
            bool isTaken;

            do
            {
                newNickname = Util.GenerateNickname();
                isTaken = IsNicknameTaken(newNickname);
            }
            while (isTaken);

            return newNickname;
        }

        public async Task<string?> GetNicknameAsync(string connectionId)
        {
            var httpContext = _connectionMappingService.GetHttpContext(connectionId);

            if (httpContext == null)
            {
                throw new Exception("httpContext was null for the given connectionId");
            }

            var user = await _userManager.GetUserAsync(httpContext.User);

            if (user == null)
            {
                // not logged in
                return null;
            }

            var nickname = user.Nickname;

            if (nickname == null)
            {
                throw new Exception("nickname was null for given user");
            }

            return nickname;
        }
    }
}
