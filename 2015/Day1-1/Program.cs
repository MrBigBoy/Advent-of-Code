var floor = 0;

var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

foreach (var c in await File.ReadAllLinesAsync(path))
{
    foreach (var ch in c)
    {
        floor += ch == '(' ? 1 : -1;
    }
}

Console.WriteLine($"Santa is on floor {floor}");
