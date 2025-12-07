using System.Text.RegularExpressions;

internal class Program
{
    private static void Main()
    {
        var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));
        var lines = File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        var pattern = new Regex("^(\\w+) would (gain|lose) (\\d+) happiness units by sitting next to (\\w+)\\.");
        var map = new Dictionary<(string A, string B), int>();
        var people = new HashSet<string>();

        foreach (var raw in lines)
        {
            var m = pattern.Match(raw.Trim());
            if (!m.Success) continue;
            string a = m.Groups[1].Value;
            string sign = m.Groups[2].Value;
            int val = int.Parse(m.Groups[3].Value);
            string b = m.Groups[4].Value;
            int delta = sign == "gain" ? val : -val;
            map[(a, b)] = delta;
            people.Add(a);
            people.Add(b);
        }

        // Add self with 0 happiness to/from everyone
        const string Me = "Me";
        foreach (var p in people)
        {
            map[(Me, p)] = 0;
            map[(p, Me)] = 0;
        }
        people.Add(Me);

        var list = people.ToList();
        int best = int.MinValue;
        foreach (var perm in Permute(list))
        {
            int total = 0;
            int n = perm.Count;
            for (int i = 0; i < n; i++)
            {
                var person = perm[i];
                var left = perm[(i - 1 + n) % n];
                var right = perm[(i + 1) % n];
                total += map.GetValueOrDefault((person, left));
                total += map.GetValueOrDefault((person, right));
            }
            if (total > best) best = total;
        }
        Console.WriteLine(best);
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
        if (i == j) return;
        (arr[i], arr[j]) = (arr[j], arr[i]);
    }
}
