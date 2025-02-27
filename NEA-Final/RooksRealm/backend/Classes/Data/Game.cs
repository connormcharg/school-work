namespace backend.Classes.Data
{
    public class Game
    {
        public int id { get; set; }
        public int playerOneId { get; set; }
        public int playerTwoId { get; set; }
        public string? gameData { get; set; }
    }
}