namespace Day5_2;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var ranges = new List<(long start, long end)>();

        var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToArray();

        foreach (var line in lines.Where(l => l.Contains('-')))
        {
            var parts = line.Split('-', StringSplitOptions.TrimEntries);
            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);
            if (start > end)
                (start, end) = (end, start);
            ranges.Add((start, end));
        }

        if (ranges.Count == 0)
        {
            Console.WriteLine("0 ingredient IDs are considered fresh.");
            return;
        }

        // Merge overlapping and contiguous ranges
        ranges.Sort((a, b) => a.start.CompareTo(b.start));
        var merged = new List<(long start, long end)>();
        var curStart = ranges[0].start;
        var curEnd = ranges[0].end;
        for (var i = 1; i < ranges.Count; i++)
        {
            var (s, e) = ranges[i];
            if (s <= curEnd + 1) // overlap or adjacent
            {
                if (e > curEnd)
                    curEnd = e;
            }
            else
            {
                merged.Add((curStart, curEnd));
                curStart = s;
                curEnd = e;
            }
        }
        merged.Add((curStart, curEnd));

        // Sum inclusive lengths
        long totalFreshIds = 0;
        foreach (var (s, e) in merged)
        {
            totalFreshIds += (e - s + 1);
        }

        Console.WriteLine("{0} ingredient IDs are considered fresh.", totalFreshIds);
    }
}
