namespace backend.Classes.Data
{
    /// <summary>
    /// Defines the <see cref="IStatisticsRepository" />
    /// </summary>
    public interface IStatisticsRepository
    {
        /// <summary>
        /// The CreateStatistic
        /// </summary>
        /// <param name="numberOfMoves">The numberOfMoves<see cref="int"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <param name="gameId">The gameId<see cref="int"/></param>
        /// <param name="outcome">The outcome<see cref="string"/></param>
        /// <param name="dateTime">The dateTime<see cref="DateTime?"/></param>
        /// <returns>The <see cref="bool"/></returns>
        bool CreateStatistic(int numberOfMoves, int userId, int gameId, string outcome, DateTime? dateTime = null);

        /// <summary>
        /// The GetStatistics
        /// </summary>
        /// <param name="daysAgo">The daysAgo<see cref="int"/></param>
        /// <param name="userId">The userId<see cref="int"/></param>
        /// <returns>The <see cref="List{Statistic}"/></returns>
        List<Statistic> GetStatistics(int daysAgo, int userId);
    }
}
