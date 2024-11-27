namespace backend.Classes.Utilities
{
    /// <summary>
    /// Defines the <see cref="ListUtilities" />
    /// </summary>
    public static class ListUtilities
    {
        /// <summary>
        /// The Copy2dList
        /// </summary>
        /// <param name="list">The list<see cref="List{List{double}}"/></param>
        /// <param name="reverse">The reverse<see cref="bool"/></param>
        /// <returns>The <see cref="List{List{double}}"/></returns>
        public static List<List<double>> Copy2dList(List<List<double>> list, bool reverse = false)
        {
            List<List<double>> newList = new List<List<double>>();

            if (reverse)
            {
                for (int i = list.Count() - 1; i >= 0; i--)
                {
                    var t = new List<double>();
                    for (int j = list[i].Count() - 1; j >= 0; j--)
                    {
                        t.Add(list[i][j]);
                    }
                    newList.Add(t);
                }
            }
            else
            {
                foreach (var t in list)
                {
                    newList.Add(t);
                }
            }

            return newList;
        }

        /// <summary>
        /// The Shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list<see cref="IList{T}"/></param>
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            Random rng = new Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
