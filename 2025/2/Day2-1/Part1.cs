namespace Day2_1;

public static class Part1
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        long sumInvalidIds = 0;

        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            var ranges = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var range in ranges)
            {
                var bounds = range.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (bounds.Length != 2 ||
                    !long.TryParse(bounds[0], out var min) ||
                    !long.TryParse(bounds[1], out var max) ||
                    max < min)
                {
                    continue;
                }

                for (var id = min; id <= max; id++)
                {
                    if (IsRepeatedTwice(id))
                    {
                        sumInvalidIds += id;
                    }
                }
            }
        }

        Console.WriteLine(sumInvalidIds);
    }

    // Invalid if digits form X+X (exactly two copies of same substring, no leading zero case already ensured by input).
    private static bool IsRepeatedTwice(long number)
    {
        var s = number.ToString();
        if (s.Length % 2 != 0)
            return false; // must split into two equal halves
        var half = s.Length / 2;
        return s.AsSpan(0, half).SequenceEqual(s.AsSpan(half, half));
    }
}
