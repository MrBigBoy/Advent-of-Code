namespace Day8_2;

public static class Program
{
    private sealed class DSU
    {
        private readonly int[] parent;
        private readonly int[] size;
        public DSU(int n)
        {
            parent = new int[n];
            size = new int[n];
            for (int i = 0; i < n; i++) { parent[i] = i; size[i] = 1; }
        }
        private int Find(int x)
        {
            if (parent[x] != x) parent[x] = Find(parent[x]);
            return parent[x];
        }
        public bool Union(int a, int b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (ra == rb) return false;
            if (size[ra] < size[rb]) (ra, rb) = (rb, ra);
            parent[rb] = ra;
            size[ra] += size[rb];
            return true;
        }
        public int ComponentsCount()
        {
            var seen = new HashSet<int>();
            for (int i = 0; i < parent.Length; i++) seen.Add(Find(i));
            return seen.Count;
        }
    }

    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var points = new List<(long x, long y, long z)>();
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 3) continue;
            var x = long.Parse(parts[0]);
            var y = long.Parse(parts[1]);
            var z = long.Parse(parts[2]);
            points.Add((x, y, z));
        }
        var n = points.Count;
        if (n == 0)
        {
            Console.WriteLine("0");
            return;
        }

        // Build all pair distances
        var edges = new List<(int a, int b, double d)>();
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                var (x1, y1, z1) = points[i];
                var (x2, y2, z2) = points[j];
                var dx = (double)(x1 - x2);
                var dy = (double)(y1 - y2);
                var dz = (double)(z1 - z2);
                var d = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                edges.Add((i, j, d));
            }
        }
        edges.Sort((e1, e2) => e1.d.CompareTo(e2.d));

        var dsu = new DSU(n);
        long result = 0;
        int components = n;
        foreach (var (a, b, _) in edges)
        {
            if (dsu.Union(a, b))
            {
                components--;
                if (components == 1)
                {
                    // This connection causes single circuit
                    result = points[a].x * points[b].x;
                    break;
                }
            }
        }

        Console.WriteLine(result);
    }
}
