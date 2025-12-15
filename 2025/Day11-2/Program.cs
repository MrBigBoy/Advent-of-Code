namespace Day11_2;

public static class Program
{
    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var lines = File.ReadAllLines(path);
        var servers = new Dictionary<string, Server>();
        foreach (var line in lines)
        {
            var parts = line.Split(": ");
            var serverId = parts[0];
            if (!servers.TryGetValue(serverId, out var value))
            {
                value = new Server() { Id = serverId };
                servers[serverId] = value;
            }
            var outputs = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            value.Outputs = outputs;
        }

        // Memoized DFS counting paths from node to 'out', tracking whether we've visited dac/fft.
        var memo = new Dictionary<(string node, int mask), long>();
        long CountPaths(string node, int mask, HashSet<string> visiting)
        {
            // mask bit0 -> visited dac, bit1 -> visited fft
            if (node == "out")
            {
                return (mask & 0b11) == 0b11 ? 1 : 0;
            }
            if (!servers.TryGetValue(node, out var srv)) return 0;
            // Detect cycles: don't revisit nodes in current path
            if (!visiting.Add(node)) return 0;

            int newMask = mask;
            if (node == "dac") newMask |= 0b01;
            if (node == "fft") newMask |= 0b10;

            var key = (node, newMask);
            // Memoization only valid if not part of a cycle; for DAGs this works. If cycles exist, visited prevents infinite recursion.
            if (memo.TryGetValue(key, out var cached)) { visiting.Remove(node); return cached; }

            long total = 0;
            foreach (var next in srv.Outputs)
            {
                total += CountPaths(next, newMask, visiting);
            }

            memo[key] = total;
            visiting.Remove(node);
            return total;
        }

        long count = CountPaths("svr", 0, new HashSet<string>());
        Console.WriteLine($"Different paths to out visiting dac and fft: {count}");
    }

    public class Server
    {
        public List<string> Outputs { get; set; } = [];
        public string Id { get; set; } = "";
    }
}
