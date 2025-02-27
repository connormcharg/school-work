namespace backend.Classes.Data
{
    public class Statistic
    {
        public int id { get; set; }
        public double avgMoveTime { get; set; }
        public int numberOfMoves { get; set; }
        public string? outcome { get; set; }
        public DateTime datetime { get; set; }
        public string? playerOneUsername { get; set; }
        public string? playerTwoUsername { get; set; }
    }
}