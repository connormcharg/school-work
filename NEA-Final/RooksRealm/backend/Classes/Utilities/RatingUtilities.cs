namespace backend.Classes.Utilities
{
    public static class RatingUtilities
    {
        public static int GetRatingChange(int r1, int r2, double score)
        {
            double expected = 1 / (Math.Pow(10, ((r2 - r1) / 400)) + 1);
            double change = 40 * (score - expected);
            return (int)change;
        }

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