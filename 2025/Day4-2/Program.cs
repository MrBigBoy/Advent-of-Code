namespace Day4_2;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToArray();
        var height = lines.Length;
        var width = height > 0 ? lines[0].Length : 0;

        // Use a mutable grid
        var grid = new char[height][];
        for (var r = 0; r < height; r++)
            grid[r] = lines[r].ToCharArray();

        var totalRemoved = 0;
        while (true)
        {
            var toRemove = new List<(int r, int c)>();
            for (var r = 0; r < height; r++)
            {
                for (var c = 0; c < width; c++)
                {
                    if (grid[r][c] != '@')
                        continue;

                    var adjacentRolls = 0;
                    for (var dr = -1; dr <= 1; dr++)
                    {
                        for (var dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0)
                                continue;
                            var nr = r + dr;
                            var nc = c + dc;
                            if (nr < 0 || nr >= height || nc < 0 || nc >= width)
                                continue;
                            if (grid[nr][nc] == '@')
                                adjacentRolls++;
                        }
                    }

                    if (adjacentRolls < 4)
                        toRemove.Add((r, c));
                }
            }

            if (toRemove.Count == 0)
                break;

            foreach (var (r, c) in toRemove)
                grid[r][c] = '.'; // remove

            totalRemoved += toRemove.Count;
        }

        Console.WriteLine("Total rolls of paper that can be removed: {0}", totalRemoved);
    }
}
