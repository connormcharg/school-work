using System.Collections.Concurrent;

namespace CheckAndMate.Services
{
    public class ConnectionMappingService
    {
        private readonly ConcurrentDictionary<string, HttpContext> _connectionHttpContexts = new ConcurrentDictionary<string, HttpContext>();

        public void AddConnection(string connectionId, HttpContext httpContext)
        {
            _connectionHttpContexts[connectionId] = httpContext;
        }

        public void RemoveConnection(string connectionId)
        {
            _connectionHttpContexts.TryRemove(connectionId, out _);
        }

        public HttpContext? GetHttpContext(string connectionId)
        {
            _connectionHttpContexts.TryGetValue(connectionId, out var httpContext);
            return httpContext;
        }
    }
}
