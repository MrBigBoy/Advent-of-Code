namespace Day9_1;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var points = new List<(long x, long y)>();
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2) continue;
            var x = long.Parse(parts[0]);
            var y = long.Parse(parts[1]);
            points.Add((x, y));
        }

        var n = points.Count;
        if (n < 2)
        {
            Console.WriteLine("0");
            return;
        }

        long maxArea = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                var dx = Math.Abs(points[i].x - points[j].x);
                var dy = Math.Abs(points[i].y - points[j].y);
                var area = (dx + 1L) * (dy + 1L);
                if (area > maxArea) maxArea = area;
            }
        }

        Console.WriteLine(maxArea);
    }
}
