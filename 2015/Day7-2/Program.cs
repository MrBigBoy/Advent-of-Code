var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

var rules = new Dictionary<string, string>();
foreach (var line in await File.ReadAllLinesAsync(path))
{
    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    var parts = line.Split(" -> ", StringSplitOptions.TrimEntries);
    rules[parts[1]] = parts[0];
}

var cache = new Dictionary<string, ushort>();

ushort Eval(string token)
{
    if (ushort.TryParse(token, out var num))
    {
        return num;
    }

    if (cache.TryGetValue(token, out var val))
    {
        return val;
    }

    var expr = rules[token];
    ushort result;

    if (expr.StartsWith("NOT "))
    {
        var op = expr[4..];
        result = (ushort)~Eval(op);
    }
    else if (expr.Contains(" AND "))
    {
        var ps = expr.Split(" AND ");
        result = (ushort)(Eval(ps[0]) & Eval(ps[1]));
    }
    else if (expr.Contains(" OR "))
    {
        var ps = expr.Split(" OR ");
        result = (ushort)(Eval(ps[0]) | Eval(ps[1]));
    }
    else if (expr.Contains(" LSHIFT "))
    {
        var ps = expr.Split(" LSHIFT ");
        result = (ushort)(Eval(ps[0]) << int.Parse(ps[1]));
    }
    else if (expr.Contains(" RSHIFT "))
    {
        var ps = expr.Split(" RSHIFT ");
        result = (ushort)(Eval(ps[0]) >> int.Parse(ps[1]));
    }
    else
    {
        result = Eval(expr);
    }

    cache[token] = result;
    return result;
}

// First evaluation to get signal on 'a'
var a1 = Eval("a");

// Override wire 'b' with a1 and reset cache, then re-evaluate 'a'
rules["b"] = a1.ToString();
cache.Clear();
var a2 = Eval("a");

Console.WriteLine(a2);
