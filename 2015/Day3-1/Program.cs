var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

// Read the full instruction string (first line)
var input = (await File.ReadAllTextAsync(path)).Trim();

// Track visited positions using a HashSet of tuples
var visited = new HashSet<(int x, int y)>();
int x = 0, y = 0;
visited.Add((x, y));

foreach (var c in input)
{
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

    visited.Add((x, y));
}

Console.WriteLine(visited.Count);
