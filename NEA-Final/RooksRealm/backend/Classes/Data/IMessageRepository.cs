namespace backend.Classes.Data
{
    public interface IMessageRepository
    {
        public List<Message> GetMessages();
        public bool CreateMessage(string title, string content, int userId);
    }
}
