namespace Day3_1;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        long totalJoltage = 0;
        const int batteryCount = 2; // Number of batteries to consider

        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            // Greedy: pick the leftmost occurrence of the highest digit available,
            // ensuring enough digits remain to reach batteryCount while preserving order.
            var picked = new char[batteryCount];
            var start = 0;
            for (var k = 0; k < batteryCount; k++)
            {
                // Last position we can start searching so that enough digits remain
                var lastSearchStart = line.Length - (batteryCount - k);
                var chosenIndex = -1;
                var chosenDigit = '\0';

                // Try digits from '9' down to '0'
                for (var d = '9'; d >= '0'; d--)
                {
                    var idx = line.IndexOf(d, start, lastSearchStart - start + 1);
                    if (idx != -1)
                    {
                        chosenIndex = idx;
                        chosenDigit = d;
                        break;
                    }
                }

                // Fallback (should not happen if input is valid): pick start
                if (chosenIndex == -1)
                {
                    chosenIndex = start;
                    chosenDigit = line[start];
                }

                picked[k] = chosenDigit;
                start = chosenIndex + 1;
            }

            var joltageStr = new string(picked);
            totalJoltage += long.Parse(joltageStr);
        }

        Console.WriteLine("Total output joltage is: {0}", totalJoltage.ToString());
    }
}
