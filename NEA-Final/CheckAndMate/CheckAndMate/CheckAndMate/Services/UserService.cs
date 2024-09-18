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

        public async Task<bool> IsNicknameTaken(string nickname)
        {
            var userWithNickname = _userManager.Users
                .FirstOrDefault(u => u.Nickname == nickname);

            return userWithNickname != null;
        }

        public async Task<string> GenerateUniqueNicknameAsync()
        {
            string newNickname;
            bool isTaken;

            do
            {
                newNickname = Util.GenerateNickname();
                isTaken = await IsNicknameTaken(newNickname);
            }
            while (isTaken);

            return newNickname;
        }

        public async Task<string> GetNicknameAsync(string connectionId)
        {
            var httpContext = _connectionMappingService.GetHttpContext(connectionId);

            if (httpContext == null)
            {
                return "http context not found for connection id";
            }

            var user = await _userManager.GetUserAsync(httpContext.User);

            if (user == null)
            {
                return "user not found for http context";
            }

            var nickname = user.Nickname;

            if (nickname == null)
            {
                return "user has no nickname";
            }

            return nickname;
        }
    }
}
