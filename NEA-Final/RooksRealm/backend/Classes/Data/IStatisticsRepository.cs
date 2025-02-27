namespace backend.Classes.Data
{
    public interface IStatisticsRepository
    {
        bool CreateStatistic(int numberOfMoves, int userId, int gameId, string outcome, DateTime? dateTime = null);

        List<Statistic> GetStatistics(int daysAgo, int userId);
    }
}