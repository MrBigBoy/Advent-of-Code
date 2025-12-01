internal class Program
{
    private static void Main(string[] args)
    {
        const int min = 0;
        const int max = 99;
        const int range = max - min + 1;
        int position = 50;

        int zeros = 0;

        string path = args.Length > 0
            ? args[0]
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        foreach (var raw in File.ReadLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0)
                continue;

            char dir = line[0];
            if (dir != 'L' && dir != 'R')
                continue;

            if (!int.TryParse(line.AsSpan(1), out int distance))
                continue;

            int delta = dir == 'L' ? -distance : distance;
            position = ((position + delta) % range + range) % range;

            if (position == 0)
                zeros++;
        }

        Console.WriteLine(zeros);
    }
}
