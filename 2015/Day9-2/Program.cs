internal class Program
{
    private static void Main()
    {
        var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));
        var lines = File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        var distances = new Dictionary<(string A, string B), int>();
        var cities = new HashSet<string>();

        foreach (var raw in lines)
        {
            // Format: City1 to City2 = N
            var parts = raw.Split(' ');
            var a = parts[0];
            var b = parts[2];
            var d = int.Parse(parts[4]);

            distances[(a, b)] = d;
            distances[(b, a)] = d; // undirected
            cities.Add(a);
            cities.Add(b);
        }

        var cityList = cities.ToList();
        int n = cityList.Count;
        int worst = int.MinValue;

        // brute-force all permutations
        foreach (var perm in Permute(cityList))
        {
            int total = 0;
            bool ok = true;
            for (int i = 0; i < n - 1; i++)
            {
                var from = perm[i];
                var to = perm[i + 1];
                if (!distances.TryGetValue((from, to), out var d))
                {
                    ok = false;
                    break;
                }
                total += d;
            }
            if (ok && total > worst)
                worst = total;
        }

        Console.WriteLine(worst);
    }

    private static IEnumerable<List<T>> Permute<T>(IList<T> items)
    {
        var arr = items.ToArray();
        return Permute(arr, 0);
    }

    private static IEnumerable<List<T>> Permute<T>(T[] arr, int start)
    {
        if (start == arr.Length - 1)
        {
            yield return arr.ToList();
            yield break;
        }
        for (int i = start; i < arr.Length; i++)
        {
            Swap(arr, start, i);
            foreach (var p in Permute(arr, start + 1))
                yield return p;
            Swap(arr, start, i);
        }
    }

    private static void Swap<T>(T[] arr, int i, int j)
    {
        if (i == j)
            return;
        (arr[i], arr[j]) = (arr[j], arr[i]);
    }
}
