namespace Day8_1;

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
        public int[] ComponentSizes()
        {
            var roots = new Dictionary<int, int>();
            for (int i = 0; i < parent.Length; i++)
            {
                var r = Find(i);
                roots[r] = roots.TryGetValue(r, out var v) ? v + 1 : 1;
            }
            return roots.Values.ToArray();
        }
    }

    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var points = new List<(double x, double y, double z)>();
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            var parts = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 3) continue;
            var x = double.Parse(parts[0]);
            var y = double.Parse(parts[1]);
            var z = double.Parse(parts[2]);
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
                var dx = x1 - x2;
                var dy = y1 - y2;
                var dz = z1 - z2;
                var d = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                edges.Add((i, j, d));
            }
        }
        edges.Sort((e1, e2) => e1.d.CompareTo(e2.d));

        var dsu = new DSU(n);
        // Connect the 1000 closest pairs (attempts), union if in different circuits
        int attempts = Math.Min(1000, edges.Count);
        for (int k = 0; k < attempts; k++)
        {
            var (a, b, _) = edges[k];
            _ = dsu.Union(a, b);
        }

        var sizes = dsu.ComponentSizes();
        Array.Sort(sizes); Array.Reverse(sizes);
        long result = 1;
        for (int i = 0; i < Math.Min(3, sizes.Length); i++)
        {
            result *= sizes[i];
        }
        Console.WriteLine(result);
    }
}
