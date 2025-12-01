const int min = 0;
const int max = 99;
const int range = max - min + 1;
var position = 50;
var zeros = 0;

var path = args.Length > 0
            ? args[0]
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

foreach (var raw in File.ReadLines(path))
{
    var line = raw.Trim();
    if (line.Length == 0)
    {
        continue;
    }

    var dir = line[0];
    if (dir is not 'L' and not 'R')
    {
        continue;
    }

    if (!int.TryParse(line.AsSpan(1), out var distance))
    {
        continue;
    }

    var delta = dir == 'L' ? -distance : distance;
    position = (((position + delta) % range) + range) % range;

    if (position == 0)
    {
        zeros++;
    }
}

Console.WriteLine(zeros);
