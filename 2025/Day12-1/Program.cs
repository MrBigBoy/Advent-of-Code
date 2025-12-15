namespace Day12_1;

using System.Linq;

public static class Program
{
    private sealed class Shape
    {
        public bool[,] Cells = new bool[0, 0];
        public int Area;

        public IEnumerable<bool[,]> Variants()
        {
            var seen = new HashSet<string>();
            foreach (var rot in new[] { 0, 1, 2, 3 })
            {
                var a = Rotate(Cells, rot);
                foreach (var flipH in new[] { false, true })
                {
                    var v = flipH ? FlipH(a) : a;
                    v = Normalize(v);
                    var key = Key(v);
                    if (seen.Add(key))
                        yield return v;
                }
            }
        }

        private static bool[,] Rotate(bool[,] src, int times)
        {
            bool[,] a = src;
            for (int t = 0; t < times; t++)
            {
                int h = a.GetLength(0), w = a.GetLength(1);
                var r = new bool[w, h];
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        r[j, h - 1 - i] = a[i, j];
                a = r;
            }
            return a;
        }

        private static bool[,] FlipH(bool[,] a)
        {
            int h = a.GetLength(0), w = a.GetLength(1);
            var r = new bool[h, w];
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    r[i, w - 1 - j] = a[i, j];
            return r;
        }

        private static bool[,] Normalize(bool[,] a)
        {
            int h = a.GetLength(0), w = a.GetLength(1);
            int minI = h, minJ = w, maxI = -1, maxJ = -1;
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    if (a[i, j])
                    { minI = Math.Min(minI, i); minJ = Math.Min(minJ, j); maxI = Math.Max(maxI, i); maxJ = Math.Max(maxJ, j); }
            if (maxI == -1)
                return new bool[1, 1];
            int nh = maxI - minI + 1, nw = maxJ - minJ + 1;
            var r = new bool[nh, nw];
            for (int i = 0; i < nh; i++)
                for (int j = 0; j < nw; j++)
                    r[i, j] = a[minI + i, minJ + j];
            return r;
        }

        private static string Key(bool[,] a)
        {
            int h = a.GetLength(0), w = a.GetLength(1);
            var sb = new System.Text.StringBuilder(h * (w + 1));
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                    sb.Append(a[i, j] ? '#' : '.');
                sb.Append('|');
            }
            return sb.ToString();
        }
    }

    private sealed class Region
    {
        public int Width;
        public int Height;
        public int[] Quantities = Array.Empty<int>();
    }

    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var lines = File.ReadAllLines(path);
        var shapes = ParseShapes(lines);
        var regions = ParseRegions(lines, shapes.Count);

        int fitCount = 0;
        foreach (var r in regions)
        {
            bool fits = CanTileRegion(r, shapes);
            Console.WriteLine($"{r.Width}x{r.Height}: {(fits ? "fits" : "no fit")}");
            if (fits)
                fitCount++;
        }
        Console.WriteLine(fitCount);
    }

    private static List<Shape> ParseShapes(string[] lines)
    {
        var shapes = new List<Shape>();
        int i = 0;
        while (i < lines.Length)
        {
            var s = lines[i].Trim();
            if (string.IsNullOrEmpty(s))
            { i++; continue; }
            if (s.EndsWith(":"))
            {
                // start of shape
                i++;
                var rows = new List<string>();
                while (i < lines.Length)
                {
                    var t = lines[i].Trim();
                    if (t.Length == 0)
                    { i++; break; }
                    if (char.IsDigit(t[0]) && t.EndsWith(":"))
                    { break; }
                    // stop when region section begins (e.g., "12x5:")
                    if (t.Contains('x') && t.EndsWith(":") && int.TryParse(t.Split('x')[0], out _))
                    { break; }
                    rows.Add(t);
                    i++;
                }
                if (rows.Count > 0)
                {
                    int h = rows.Count, w = rows[0].Length;
                    var shape = new Shape { Cells = new bool[h, w] };
                    int area = 0;
                    for (int r = 0; r < h; r++)
                    {
                        for (int c = 0; c < w; c++)
                        {
                            bool v = rows[r][c] == '#';
                            shape.Cells[r, c] = v;
                            if (v)
                                area++;
                        }
                    }
                    shape.Cells = NormalizeShape(shape.Cells);
                    shape.Area = area;
                    shapes.Add(shape);
                }
            }
            else
                i++;
        }
        return shapes;
    }

    private static bool[,] NormalizeShape(bool[,] a)
    {
        int h = a.GetLength(0), w = a.GetLength(1);
        int minI = h, minJ = w, maxI = -1, maxJ = -1;
        for (int i = 0; i < h; i++)
            for (int j = 0; j < w; j++)
                if (a[i, j])
                { minI = Math.Min(minI, i); minJ = Math.Min(minJ, j); maxI = Math.Max(maxI, i); maxJ = Math.Max(maxJ, j); }
        if (maxI == -1)
            return new bool[1, 1];
        int nh = maxI - minI + 1, nw = maxJ - minJ + 1;
        var r = new bool[nh, nw];
        for (int i = 0; i < nh; i++)
            for (int j = 0; j < nw; j++)
                r[i, j] = a[minI + i, minJ + j];
        return r;
    }

    private sealed record Placement(List<int> Cells);

    private static List<Region> ParseRegions(string[] lines, int shapeCount)
    {
        var result = new List<Region>();
        foreach (var line in lines)
        {
            var s = line.Trim();
            if (s.Length == 0)
                continue;

            // Expect format: WxH: q0 q1 ...
            var parts = s.Split(':');
            if (parts.Length >= 2 && parts[0].Contains('x'))
            {
                var dims = parts[0].Split('x');
                if (dims.Length == 2 && int.TryParse(dims[0], out int w) && int.TryParse(dims[1], out int h))
                {
                    var qtyTokens = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (qtyTokens.Length == shapeCount)
                    {
                        var qty = new int[shapeCount];
                        bool ok = true;
                        for (int i = 0; i < shapeCount; i++)
                        {
                            if (!int.TryParse(qtyTokens[i], out qty[i]))
                            { ok = false; break; }
                        }
                        if (ok)
                        {
                            result.Add(new Region { Width = w, Height = h, Quantities = qty });
                        }
                    }
                }
            }
        }
        return result;
    }

    private static bool CanTileRegion(Region region, List<Shape> shapes)
    {
        int W = region.Width, H = region.Height;
        int totalAreaNeeded = 0;
        for (int i = 0; i < shapes.Count; i++)
            totalAreaNeeded += region.Quantities[i] * shapes[i].Area;
        if (totalAreaNeeded > W * H)
            return false; // quick fail: can't exceed area

        // Precompute placements for each shape index
        var shapePlacements = new List<List<Placement>>();
        shapePlacements.Capacity = shapes.Count;
        for (int si = 0; si < shapes.Count; si++)
        {
            var placements = new List<Placement>();
            var variants = shapes[si].Variants().ToList();
            foreach (var v in variants)
            {
                int vh = v.GetLength(0), vw = v.GetLength(1);
                for (int oy = 0; oy <= H - vh; oy++)
                {
                    for (int ox = 0; ox <= W - vw; ox++)
                    {
                        var cells = new List<int>();
                        bool ok = true;
                        for (int y = 0; y < vh && ok; y++)
                        {
                            for (int x = 0; x < vw; x++)
                            {
                                if (!v[y, x])
                                    continue;
                                int cy = oy + y, cx = ox + x;
                                if (cy < 0 || cy >= H || cx < 0 || cx >= W)
                                { ok = false; break; }
                                cells.Add(cy * W + cx);
                            }
                        }
                        if (ok && cells.Count > 0)
                        {
                            placements.Add(new Placement(cells));
                        }
                    }
                }
            }
            shapePlacements.Add(placements);
        }

        // Early fail: if any needed shape has no placements
        for (int i = 0; i < shapes.Count; i++)
        {
            if (region.Quantities[i] > 0 && shapePlacements[i].Count == 0)
                return false;
        }

        // Order shapes by difficulty (fewest placements first, then larger area first)
        var order = Enumerable.Range(0, shapes.Count)
            .Where(i => region.Quantities[i] > 0)
            .OrderBy(i => shapePlacements[i].Count)
            .ThenByDescending(i => shapes[i].Area)
            .ToArray();

        var occupied = new bool[W * H];
        int freeCells = W * H;

        bool PlaceRec(int idx, int[] remaining, int remainingArea)
        {
            // Prune: not enough free area left
            if (remainingArea > freeCells)
                return false;
            if (idx == order.Length)
                return true;
            int si = order[idx];
            if (remaining[si] == 0)
                return PlaceRec(idx + 1, remaining, remainingArea);

            // Try placements for this shape
            foreach (var p in shapePlacements[si])
            {
                // Check overlap
                bool overlap = false;
                foreach (var cell in p.Cells)
                {
                    if (occupied[cell])
                    { overlap = true; break; }
                }
                if (overlap)
                    continue;

                // Place
                foreach (var cell in p.Cells)
                { occupied[cell] = true; }
                freeCells -= p.Cells.Count;
                remaining[si]--;
                if (PlaceRec(idx, remaining, remainingArea - shapes[si].Area))
                    return true; // still need to place more of this shape

                // Backtrack
                remaining[si]++;
                foreach (var cell in p.Cells)
                { occupied[cell] = false; }
                freeCells += p.Cells.Count;
            }
            return false;
        }

        var remainingArr = (int[])region.Quantities.Clone();
        return PlaceRec(0, remainingArr, totalAreaNeeded);
    }
}
