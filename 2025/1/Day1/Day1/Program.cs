public static class Program
{
    public static void Main(string[] args)
    {
        // Usage: dotnet run -- [part] [inputPath]
        // part: "1"|"part1" or "2"|"part2" (defaults to 1)
        // inputPath: optional; if omitted, uses ../../../input.txt

        if (args.Length == 0)
        {
            Part1.Run(Array.Empty<string>());
            return;
        }

        var partArg = args[0].ToLowerInvariant();
        if (partArg is "1" or "part1")
        {
            var rest = args.Length > 1 ? args[1..] : Array.Empty<string>();
            Part1.Run(rest);
        }
        else if (partArg is "2" or "part2")
        {
            var rest = args.Length > 1 ? args[1..] : Array.Empty<string>();
            Part2.Run(rest);
        }
        else
        {
            // If the first arg isn't a part selector, assume it's a path for Part1
            Part1.Run(args);
        }
    }
}
