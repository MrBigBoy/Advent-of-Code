var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

// Read the full instruction string (first line)
var input = (await File.ReadAllTextAsync(path)).Trim();

// Track visited positions using a HashSet of tuples
var visited = new HashSet<(int x, int y)>();
int sx = 0, sy = 0; // Santa
int rx = 0, ry = 0; // Robo-Santa
visited.Add((0, 0));

for (var i = 0; i < input.Length; i++)
{
    var c = input[i];
    var santaTurn = (i % 2) == 0;

    ref var x = ref santaTurn ? ref sx : ref rx;
    ref var y = ref santaTurn ? ref sy : ref ry;

    switch (c)
    {
        case '^':
            y += 1;
            break;

        case 'v':
            y -= 1;
            break;

        case '>':
            x += 1;
            break;

        case '<':
            x -= 1;
            break;

        default:

            // ignore any unexpected characters
            continue;
    }

    _ = visited.Add((x, y));
}

Console.WriteLine(visited.Count);
