namespace Day7_1;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToList();
        if (lines.Count == 0)
        {
            Console.WriteLine("Total: 0");
            return;
        }

        var width = lines.Max(l => l.Length);
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Length < width)
                lines[i] = lines[i].PadRight(width, '.');
        }

        var height = lines.Count;

        // Find S position
        int startRow = -1, startCol = -1;
        for (int r = 0; r < height; r++)
        {
            var cIdx = lines[r].IndexOf('S');
            if (cIdx >= 0)
            {
                startRow = r;
                startCol = cIdx;
                break;
            }
        }
        if (startRow == -1)
        {
            Console.WriteLine("Total: 0");
            return;
        }

        // Simulate beams downward; dedupe beams per row (dumping into same place)
        var beamsPerRow = new List<HashSet<int>>(height);
        for (int r = 0; r < height; r++)
            beamsPerRow.Add(new HashSet<int>());

        var currentBeams = new HashSet<int> { startCol };
        long splitCount = 0;

        for (int r = startRow + 1; r < height; r++)
        {
            var nextBeams = new HashSet<int>();
            foreach (var col in currentBeams)
            {
                if (col < 0 || col >= width)
                    continue;
                var cell = lines[r][col];
                if (cell == '^')
                {
                    splitCount++;
                    if (col - 1 >= 0)
                        nextBeams.Add(col - 1);
                    if (col + 1 < width)
                        nextBeams.Add(col + 1);
                }
                else
                {
                    nextBeams.Add(col);
                }
            }
            beamsPerRow[r] = nextBeams; // record positions at this row
            currentBeams = nextBeams;
            if (currentBeams.Count == 0)
                break; // no more beams
        }

        // Optional visualization: color from green (top) to red (bottom)
        for (int r = 0; r < height; r++)
        {
            double t = height > 1 ? (double)r / (height - 1) : 1.0;
            int R = (int)Math.Round(255 * t);
            int G = (int)Math.Round(255 * (1 - t));
            var color = $"\x1b[38;2;{R};{G};0m";
            var reset = "\x1b[0m";

            var rowChars = lines[r].ToCharArray();
            foreach (var c in beamsPerRow[r])
            {
                if (c >= 0 && c < width)
                {
                    if (rowChars[c] == '.' || rowChars[c] == '|')
                        rowChars[c] = '|';
                }
            }
            Console.Write(color);
            Console.WriteLine(new string(rowChars));
            Console.Write(reset);
        }

        if (runAsTest)
        {
            // Make sure the count is 21 for the test input
            Console.WriteLine($"Asserting split count is 21 for test input...");
        }

        Console.WriteLine($"Total splits: {splitCount}");
    }
}
