using Day6_2;

var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

var lights = new int[1000, 1000];

foreach (var line in await File.ReadAllLinesAsync(path))
{
    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    Operation op;
    string coords;

    if (line.StartsWith("turn on"))
    {
        op = Operation.On;
        coords = line["turn on".Length..].Trim();
    }
    else if (line.StartsWith("turn off"))
    {
        op = Operation.Off;
        coords = line["turn off".Length..].Trim();
    }
    else if (line.StartsWith("toggle"))
    {
        op = Operation.Toggle;
        coords = line["toggle".Length..].Trim();
    }
    else
    {
        continue;
    }

    // coords format: "x1,y1 through x2,y2"
    var parts = coords.Split(" through ", StringSplitOptions.TrimEntries);
    var p1 = parts[0].Split(',');
    var p2 = parts[1].Split(',');

    var x1 = int.Parse(p1[0]);
    var y1 = int.Parse(p1[1]);
    var x2 = int.Parse(p2[0]);
    var y2 = int.Parse(p2[1]);

    if (x1 > x2)
    {
        (x1, x2) = (x2, x1);
    }

    if (y1 > y2)
    {
        (y1, y2) = (y2, y1);
    }

    for (var x = x1; x <= x2; x++)
    {
        for (var y = y1; y <= y2; y++)
        {
            switch (op)
            {
                case Operation.On:
                    lights[x, y] += 1;
                    break;

                case Operation.Off:
                    if (lights[x, y] > 0)
                    {
                        lights[x, y] -= 1;
                    }

                    break;

                case Operation.Toggle:
                    lights[x, y] += 2;
                    break;
            }
        }
    }
}

long totalBrightness = 0;
for (var x = 0; x < 1000; x++)
{
    for (var y = 0; y < 1000; y++)
    {
        totalBrightness += lights[x, y];
    }
}

Console.WriteLine(totalBrightness);

namespace Day6_2
{
    internal enum Operation
    {
        On,
        Off,
        Toggle
    }
}
