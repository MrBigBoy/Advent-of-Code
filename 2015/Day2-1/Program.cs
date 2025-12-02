var surface = 0;

var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

foreach (var c in await File.ReadAllLinesAsync(path))
{
    var dimensions = c.Split('x').Select(int.Parse).ToArray();
    var l = dimensions[0];
    var w = dimensions[1];
    var h = dimensions[2];

    var lw = l * w;
    var wh = w * h;
    var hl = h * l;

    surface += 2 * (lw + wh + hl) + Math.Min(lw, Math.Min(wh, hl));
}

Console.WriteLine($"Total surface area: {surface}");
