namespace backend.Classes.Data
{
    public class Message
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? username { get; set; }
        public DateTime datetime { get; set; }
    }
}