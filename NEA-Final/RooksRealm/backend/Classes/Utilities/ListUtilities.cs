namespace backend.Classes.Utilities
{
    public static class ListUtilities
    {
        public static List<List<double>> Copy2dList(List<List<double>> list, bool reverse = false)
        {
            List<List<double>> newList = new List<List<double>>();

            if (reverse)
            {
                for (int i = list.Count() - 1; i > 0; i--)
                {
                    var t = new List<double>();
                    for (int j = list[i].Count(); j > 0; j--)
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
