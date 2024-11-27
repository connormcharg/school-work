namespace backend.Services
{
    using System.Collections.Concurrent;

    /// <summary>
    /// Defines the <see cref="ConnectionMappingService" />
    /// </summary>
    public class ConnectionMappingService
    {
        /// <summary>
        /// Defines the connectionHttpContexts
        /// </summary>
        private readonly ConcurrentDictionary<string, HttpContext> connectionHttpContexts =
            new ConcurrentDictionary<string, HttpContext>();

        /// <summary>
        /// The AddConnection
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <param name="context">The context<see cref="HttpContext"/></param>
        public void AddConnection(string id, HttpContext context)
        {
            connectionHttpContexts[id] = context;
        }

        /// <summary>
        /// The RemoveConnection
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        public void RemoveConnection(string id)
        {
            connectionHttpContexts.TryRemove(id, out _);
        }

        /// <summary>
        /// The GetHttpContext
        /// </summary>
        /// <param name="id">The id<see cref="string"/></param>
        /// <returns>The <see cref="HttpContext?"/></returns>
        public HttpContext? GetHttpContext(string id)
        {
            connectionHttpContexts.TryGetValue(id, out var context);
            return context;
        }
    }
}
