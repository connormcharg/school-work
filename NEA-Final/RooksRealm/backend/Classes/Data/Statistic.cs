namespace backend.Classes.Data
{
    public class Statistic
    {
        public int id { get; set; }
        public double avgMoveTime { get; set; }
        public int numberOfMoves { get; set; }
        public int userId { get; set; }
        public int gameId { get; set; }
        public string? outcome { get; set; }
    }
}
