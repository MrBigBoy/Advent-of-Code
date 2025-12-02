namespace Day2_2;

public static class Part2
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
                    if (IsMadeOfRepeatedSequence(id))
                    {
                        sumInvalidIds += id;
                    }
                }
            }
        }

        Console.WriteLine(sumInvalidIds);
    }

    // Invalid if digits is made of some sequence of digits
    private static bool IsMadeOfRepeatedSequence(long number)
    {
        var s = number.ToString();
        var len = s.Length;
        for (var seqLen = 1; seqLen <= len / 2; seqLen++)
        {
            if (len % seqLen != 0)
                continue; // sequence length must divide total length
            var sequence = s.AsSpan(0, seqLen);
            var isValid = true;
            for (var pos = seqLen; pos < len; pos += seqLen)
            {
                if (!sequence.SequenceEqual(s.AsSpan(pos, seqLen)))
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid)
                return true;
        }
        return false;
    }
}
