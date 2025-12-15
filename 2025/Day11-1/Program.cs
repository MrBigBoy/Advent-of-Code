namespace Day11_1;

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
                value = new Server()
                {
                    Id = serverId
                };
                servers[serverId] = value;
            }
            var outputs = parts[1].Split(" ").ToList();
            value.Outputs = outputs;
        }

        var differentPaths = new HashSet<string>();

        // Start with the server you and check the output of that.
        var serverYou = servers["you"];
        var toCheck = new Queue<(Server server, List<string> path)>();
        toCheck.Enqueue((serverYou, new List<string> { serverYou.Id }));
        while (toCheck.Count > 0)
        {
            var current = toCheck.Dequeue();
            var server = current.server;
            var pathSoFar = current.path;
            foreach (var output in server.Outputs)
            {
                if (output == "out")
                {
                    // Found out, now add it to the different paths with all its parents. So the path will be from "you" to "out". Like you->bbb->ccc->out.
                    var fullPath = string.Join("->", pathSoFar.Append("out"));
                    differentPaths.Add(fullPath);
                    continue;
                }
                if (!servers.TryGetValue(output, out var nextServer))
                    continue;

                // Avoid cycles in the current path
                if (pathSoFar.Contains(nextServer.Id))
                    continue;
                var nextPath = new List<string>(pathSoFar) { nextServer.Id };
                toCheck.Enqueue((nextServer, nextPath));
            }
        }

        Console.WriteLine($"Different paths to out: {differentPaths.Count}");
    }

    public class Server
    {
        public List<string> Outputs { get; set; } = [];
        public string Id { get; set; } = "";
        public bool Checked { get; set; } = false;
    }
}
