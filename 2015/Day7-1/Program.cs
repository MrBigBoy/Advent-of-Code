var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

var rules = new Dictionary<string, string>();
foreach (var line in await File.ReadAllLinesAsync(path))
{
    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    // format: "expr -> target"
    var parts = line.Split(" -> ", StringSplitOptions.TrimEntries);
    rules[parts[1]] = parts[0];
}

var cache = new Dictionary<string, ushort>();

ushort Eval(string token)
{
    // If token is a number
    if (ushort.TryParse(token, out var num))
    {
        return num;
    }

    // Memoized wire
    if (cache.TryGetValue(token, out var val))
    {
        return val;
    }

    var expr = rules[token];
    ushort result;

    // Parse expression by operation keywords
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
        // direct assignment from wire or literal
        result = Eval(expr);
    }

    cache[token] = result;
    return result;
}

Console.WriteLine(Eval("a"));
