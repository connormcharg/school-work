namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="IMessageRepository" />
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// The GetMessages
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <returns>The <see cref="List{Message}"/></returns>
        List<Message> GetMessages(int daysAgo);

        /// <summary>
        /// The CreateMessage
        /// </summary>
        /// <param name="title">The title<see cref="string"/></param>
        /// <param name="content">The content<see cref="string"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <param name="dateTime">The dateTime<see cref="DateTime?"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool CreateMessage(string title, string content, int userId, DateTime? dateTime = null);

        /// <summary>
        /// The DeleteMessage
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool DeleteMessage(int id);
    }
}
