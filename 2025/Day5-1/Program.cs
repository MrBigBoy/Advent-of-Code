namespace Day5_1;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var ranges = new List<(long start, long end)>();
        var ingredients = new List<long>();

        var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToArray();
        foreach (var line in lines)
        {
            if (line.Contains('-'))
            {
                var parts = line.Split('-', StringSplitOptions.TrimEntries);
                var start = long.Parse(parts[0]);
                var end = long.Parse(parts[1]);
                if (start > end)
                    (start, end) = (end, start);
                ranges.Add((start, end));
            }
            else
            {
                ingredients.Add(long.Parse(line));
            }
        }

        var freshCount = 0;
        foreach (var id in ingredients)
        {
            var fresh = false;
            foreach (var (start, end) in ranges)
            {
                if (id >= start && id <= end)
                {
                    fresh = true;
                    break;
                }
            }
            if (fresh)
                freshCount++;
        }

        Console.WriteLine("{0} ingredients are fresh.", freshCount);
    }
}
