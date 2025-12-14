namespace Day9_2;

public static class Program
{
    private static List<(long x, long y)> ReadPoints(string path)
    {
        var pts = new List<(long x, long y)>();
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 2) continue;
            var x = long.Parse(parts[0]);
            var y = long.Parse(parts[1]);
            pts.Add((x, y));
        }
        return pts;
    }

    private sealed class Segment
    {
        public long x1, y1, x2, y2; // axis-aligned
        public Segment(long x1, long y1, long x2, long y2)
        {
            this.x1 = x1; this.y1 = y1; this.x2 = x2; this.y2 = y2;
            if (x1 == x2)
            {
                if (y1 > y2) (y1, y2) = (y2, y1);
                this.y1 = y1; this.y2 = y2;
            }
            else
            {
                if (x1 > x2) (x1, x2) = (x2, x1);
                this.x1 = x1; this.x2 = x2;
            }
        }
        public bool IsVertical => x1 == x2;
    }

    private static List<Segment> BuildEdges(List<(long x, long y)> pts)
    {
        var edges = new List<Segment>(pts.Count);
        for (int i = 0; i < pts.Count; i++)
        {
            var a = pts[i];
            var b = pts[(i + 1) % pts.Count];
            edges.Add(new Segment(a.x, a.y, b.x, b.y));
        }
        return edges;
    }

    private static bool PointOnSegment(long x, long y, Segment s)
    {
        if (s.IsVertical)
        {
            return x == s.x1 && y >= s.y1 && y <= s.y2;
        }
        else
        {
            return y == s.y1 && x >= s.x1 && x <= s.x2;
        }
    }

    private static bool PointInsideOrOn((long x, long y) p, List<Segment> edges)
    {
        foreach (var e in edges)
        {
            if (PointOnSegment(p.x, p.y, e)) return true;
        }
        int crossings = 0;
        foreach (var e in edges)
        {
            if (!e.IsVertical) continue;
            if (p.x < e.x1 && p.y >= e.y1 && p.y < e.y2) crossings++;
        }
        return (crossings % 2) == 1;
    }

    private static List<(long start, long end)> InsideIntervalsAtY(long y, List<Segment> edges)
    {
        var xs = new List<long>();
        foreach (var e in edges)
        {
            if (!e.IsVertical) continue;
            // Use half-open [y1, y2) for crossing parity
            if (y >= e.y1 && y < e.y2) xs.Add(e.x1);
        }
        xs.Sort();
        var intervals = new List<(long start, long end)>();
        for (int i = 0; i + 1 < xs.Count; i += 2)
        {
            intervals.Add((xs[i], xs[i + 1]));
        }
        return intervals;
    }

    private static List<(long start, long end)> InsideIntervalsAtX(long x, List<Segment> edges)
    {
        var ys = new List<long>();
        foreach (var e in edges)
        {
            if (e.IsVertical) continue;
            if (x >= e.x1 && x < e.x2) ys.Add(e.y1);
        }
        ys.Sort();
        var intervals = new List<(long start, long end)>();
        for (int i = 0; i + 1 < ys.Count; i += 2)
        {
            intervals.Add((ys[i], ys[i + 1]));
        }
        return intervals;
    }

    private static bool IntervalContains(List<(long start, long end)> intervals, long a, long b)
    {
        if (a > b) (a, b) = (b, a);
        foreach (var (s, e) in intervals)
        {
            if (a >= s && b <= e) return true;
        }
        return false;
    }

    private static bool RectangleValid((long x, long y) a, (long x, long y) b, List<Segment> poly)
    {
        var x1 = Math.Min(a.x, b.x);
        var x2 = Math.Max(a.x, b.x);
        var y1 = Math.Min(a.y, b.y);
        var y2 = Math.Max(a.y, b.y);

        var c3 = (a.x, b.y);
        var c4 = (b.x, a.y);
        if (!PointInsideOrOn(c3, poly)) return false;
        if (!PointInsideOrOn(c4, poly)) return false;

        // Both horizontal edges must lie inside/on
        var intervalsBottom = InsideIntervalsAtY(y1, poly);
        var intervalsTop = InsideIntervalsAtY(y2, poly);
        if (!IntervalContains(intervalsBottom, x1, x2)) return false;
        if (!IntervalContains(intervalsTop, x1, x2)) return false;

        // Both vertical edges must lie inside/on
        var intervalsLeft = InsideIntervalsAtX(x1, poly);
        var intervalsRight = InsideIntervalsAtX(x2, poly);
        if (!IntervalContains(intervalsLeft, y1, y2)) return false;
        if (!IntervalContains(intervalsRight, y1, y2)) return false;

        return true;
    }

    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var points = ReadPoints(path);
        var n = points.Count;
        if (n < 2)
        {
            Console.WriteLine("0");
            return;
        }
        var edges = BuildEdges(points);

        long maxArea = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                var a = points[i];
                var b = points[j];
                if (!RectangleValid(a, b, edges)) continue;
                var dx = Math.Abs(a.x - b.x);
                var dy = Math.Abs(a.y - b.y);
                var area = (dx + 1L) * (dy + 1L);
                if (area > maxArea) maxArea = area;
            }
        }

        Console.WriteLine(maxArea);
    }
}
