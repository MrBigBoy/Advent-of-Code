namespace Day6_1;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToArray();

        long[,] grid = null;

        long total = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var s = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            grid ??= new long[s.Length, lines.Length];

            if (!line.StartsWith('*') && !line.StartsWith('+'))
            {
                for (var j = 0; j < s.Length; j++)
                {
                    grid[j, i] = long.Parse(s[j]);
                }
            }
            else
            {
                for (var j = 0; j < s.Length; j++)
                {
                    var op = s[j];

                    // Calculate based on the op
                    if (op == "*")
                    {
                        long prod = 1;
                        for (var k = 0; k < lines.Length - 1; k++)
                        {
                            prod *= grid[j, k];
                        }
                        total += prod;
                    }
                    else if (op == "+")
                    {
                        long sum = 0;
                        for (var k = 0; k < lines.Length - 1; k++)
                        {
                            sum += grid[j, k];
                        }
                        total += sum;
                    }
                }
            }
        }

        Console.WriteLine($"Total: {total}");
    }
}
