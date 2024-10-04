using System.Collections.Concurrent;

namespace backend.Services
{
    public class ConnectionMappingService
    {
        private readonly ConcurrentDictionary<string, HttpContext> connectionHttpContexts = 
            new ConcurrentDictionary<string, HttpContext>();

        public void AddConnection(string id, HttpContext context)
        {
            connectionHttpContexts[id] = context;
        }

        public void RemoveConnection(string id)
        {
            connectionHttpContexts.TryRemove(id, out _);
        }

        public HttpContext? GetHttpContext(string id)
        {
            connectionHttpContexts.TryGetValue(id, out var context);
            return context;
        }
    }
}
