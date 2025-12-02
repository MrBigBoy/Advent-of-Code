var ribbonLength = 0;

var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

foreach (var c in await File.ReadAllLinesAsync(path))
{
    var dimensions = c.Split('x').Select(int.Parse).ToArray();
    var l = dimensions[0];
    var w = dimensions[1];
    var h = dimensions[2];

    // Smallest perimeter uses the two smallest dimensions
    Array.Sort(dimensions); // ascending: dimensions[0] and dimensions[1] are the smallest
    var r = dimensions[0] * 2 + dimensions[1] * 2;
    var bow = l * w * h;
    ribbonLength += r + bow;
}

Console.WriteLine($"Total ribbon length: {ribbonLength}");
