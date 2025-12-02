var floor = 0;
var position = 1;

var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

foreach (var c in await File.ReadAllLinesAsync(path))
{
    foreach (var ch in c)
    {
        floor += ch == '(' ? 1 : -1;
        if (floor == -1)
        {
            Console.WriteLine($"Santa first enters the basement at position {position}");
            break;
        }
        position++;
    }
}
