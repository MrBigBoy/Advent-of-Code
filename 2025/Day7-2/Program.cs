namespace Day7_2;

using System.Numerics;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToList();
        if (lines.Count == 0)
        {
            Console.WriteLine("Total timelines: 0");
            return;
        }

        var width = lines.Max(l => l.Length);
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Length < width) lines[i] = lines[i].PadRight(width, '.');
        }
        var height = lines.Count;

        // Find S
        int startRow = -1, startCol = -1;
        for (int r = 0; r < height; r++)
        {
            var idx = lines[r].IndexOf('S');
            if (idx >= 0) { startRow = r; startCol = idx; break; }
        }
        if (startRow == -1)
        {
            Console.WriteLine("Total timelines: 0");
            return;
        }

        // DP of timelines counts per column at the current row using BigInteger to avoid overflow
        var current = new Dictionary<int, BigInteger> { [startCol] = BigInteger.One };

        for (int r = startRow + 1; r < height; r++)
        {
            var next = new Dictionary<int, BigInteger>();
            foreach (var kvp in current)
            {
                int c = kvp.Key;
                var count = kvp.Value;
                if (c < 0 || c >= width) continue;
                var cell = lines[r][c];
                if (cell == '^')
                {
                    // Split: beam stops; create left and right beams at this row
                    if (c - 1 >= 0)
                    {
                        next[c - 1] = next.TryGetValue(c - 1, out var v1) ? v1 + count : count;
                    }
                    if (c + 1 < width)
                    {
                        next[c + 1] = next.TryGetValue(c + 1, out var v2) ? v2 + count : count;
                    }
                }
                else
                {
                    // Continue downward in same column
                    next[c] = next.TryGetValue(c, out var v) ? v + count : count;
                }
            }
            current = next;
            if (current.Count == 0) break; // all timelines terminated early
        }

        BigInteger totalTimelines = BigInteger.Zero;
        foreach (var kvp in current) totalTimelines += kvp.Value;
        Console.WriteLine($"Total timelines: {totalTimelines}");
    }
}
