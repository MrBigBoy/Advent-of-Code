namespace Day6_2;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        // Read all lines including operator row; pad to equal width
        var raw = File.ReadAllLines(path);
        var lines = raw.ToList();
        var width = lines.Max(l => l.Length);
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Length < width)
                lines[i] = lines[i].PadRight(width);
        }

        var rowsCount = lines.Count;
        var numbersRows = rowsCount - 1; // last row has operators
        var operatorsRow = lines[rowsCount - 1];

        long total = 0;

        // Scan right-to-left by columns; problems are spans of non-space columns separated by columns of spaces
        int col = width - 1;
        while (col >= 0)
        {
            // Skip separator columns (all spaces)
            bool isSeparator = true;
            for (int r = 0; r < rowsCount; r++)
            {
                if (lines[r][col] != ' ')
                { isSeparator = false; break; }
            }
            if (isSeparator)
            {
                col--;
                continue;
            }

            // End of a problem span at 'col'; find span start to the left until a separator column
            int endCol = col;
            int startCol = col;
            while (startCol >= 0)
            {
                bool sep = true;
                for (int r = 0; r < rowsCount; r++)
                {
                    if (lines[r][startCol] != ' ')
                    { sep = false; break; }
                }
                if (sep)
                    break;
                startCol--;
            }
            startCol++; // move to first non-separator column

            // Find operator within span
            char op = ' ';
            for (int c = endCol; c >= startCol; c--)
            {
                var ch = operatorsRow[c];
                if (ch == '*' || ch == '+')
                { op = ch; break; }
            }

            // Build numbers for each column in the span, right-to-left
            var numbers = new List<long>();
            for (int c = endCol; c >= startCol; c--)
            {
                var sb = new System.Text.StringBuilder();
                for (int r = 0; r < numbersRows; r++)
                {
                    var ch = lines[r][c];
                    if (ch >= '0' && ch <= '9')
                        sb.Append(ch);

                    // ignore spaces
                }
                if (sb.Length == 0)
                {
                    // no digits in this column, treat as 0 (skip)
                    continue;
                }
                var val = long.Parse(sb.ToString());
                numbers.Add(val);
            }

            if (op == '*')
            {
                long prod = 1;
                foreach (var n in numbers)
                    prod *= n;
                total += prod;
            }
            else if (op == '+')
            {
                long sum = 0;
                foreach (var n in numbers)
                    sum += n;
                total += sum;
            }

            // Move left to next span
            col = startCol - 1;
        }

        Console.WriteLine($"Total: {total}");
    }
}
