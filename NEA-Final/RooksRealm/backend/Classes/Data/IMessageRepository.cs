namespace backend.Classes.Data
{
    public interface IMessageRepository
    {
        List<Message> GetMessages(int daysAgo);
        bool CreateMessage(string title, string content, int userId, DateTime? dateTime = null);
    }
}
