namespace backend.Classes.Data
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool emailConfirmed { get; set; }
        public string passwordHash { get; set; }
    }
}
