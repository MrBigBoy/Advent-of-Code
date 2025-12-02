namespace Day1_2;

public static class Program
{
    public static void Main()
    {
        const int min = 0;
        const int max = 99;
        const int range = max - min + 1;
        var position = 50;
        var zeros = 0;
        var runAsTest = false;

        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0)
            {
                continue;
            }

            var dir = line[0];
            if (dir is not 'L' and not 'R')
            {
                continue;
            }

            if (!int.TryParse(line.AsSpan(1), out var distance))
            {
                continue;
            }

            zeros += CountZerosDuringRotation(position, dir, distance, range);

            var delta = dir == 'L' ? -distance : distance;
            position = (((position + delta) % range) + range) % range;
        }

        Console.WriteLine(zeros);
    }

    private static int CountZerosDuringRotation(int position, char dir, int distance, int range)
    {
        if (distance <= 0)
        {
            return 0;
        }

        int t0;
        if (dir == 'R')
        {
            t0 = (range - position) % range;
            if (t0 == 0)
            {
                t0 = range; // next time hitting 0 when moving right
            }
        }
        else
        {
            t0 = position % range;
            if (t0 == 0)
            {
                t0 = range; // next time hitting 0 when moving left
            }
        }

        return distance < t0 ? 0 : 1 + ((distance - t0) / range);
    }
}
