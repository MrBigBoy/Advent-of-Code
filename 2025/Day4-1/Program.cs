namespace Day4_1;

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

        var total = 0;
        for (var r = 0; r < height; r++)
        {
            var row = lines[r];
            for (var c = 0; c < width; c++)
            {
                if (row[c] != '@')
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
                        if (lines[nr][nc] == '@')
                            adjacentRolls++;
                    }
                }

                if (adjacentRolls < 4)
                    total++;
            }
        }

        Console.WriteLine("Rolls of paper that can be accessed by the forklift: {0}", total);
    }
}
