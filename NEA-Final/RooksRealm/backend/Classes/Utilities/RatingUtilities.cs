namespace backend.Classes.Utilities
{
    /// <summary>
    /// Defines the <see cref="RatingUtilities" />
    /// </summary>
    public static class RatingUtilities
    {
        /// <summary>
        /// The GetRatingChange
        /// </summary>
        /// <param name="r1">The r1<see cref="int"/></param>
        /// <param name="r2">The r2<see cref="int"/></param>
        /// <param name="score">The score<see cref="double"/></param>
        /// <returns>The <see cref="int"/></returns>
        public static int GetRatingChange(int r1, int r2, double score)
        {
            double expected = 1 / (Math.Pow(10, ((r2 - r1) / 400)) + 1);
            double change = 40 * (score - expected);
            return (int)change;
        }

        /// <summary>
        /// The GetRatingChangeString
        /// </summary>
        /// <param name="newRating">The newRating<see cref="int"/></param>
        /// <param name="ratingChange">The ratingChange<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetRatingChangeString(int newRating, int ratingChange)
        {
            if (ratingChange == 0)
            {
                return $"{newRating} (=)";
            }
            if (ratingChange > 0)
            {
                return $"{newRating} (+{ratingChange})";
            }
            return $"{newRating} ({ratingChange})";
        }
    }
}
