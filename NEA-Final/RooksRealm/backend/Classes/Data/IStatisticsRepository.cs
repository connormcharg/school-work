namespace backend.Classes.Data
{
    public interface IStatisticsRepository
    {
        bool CreateStatistic(int numberOfMoves, int userId, int gameId, string outcome);
    }
}
