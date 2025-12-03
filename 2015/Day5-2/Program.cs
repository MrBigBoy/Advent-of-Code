var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

var nice = 0;
foreach (var s in await File.ReadAllLinesAsync(path))
{
    if (!IsNiceNewRules(s))
    {
        continue;
    }
    nice++;
}

Console.WriteLine(nice);

static bool IsNiceNewRules(string s)
{
    // Rule 1: pair appears at least twice without overlapping
    var hasPairTwice = false;
    var lastIndex = new Dictionary<string, int>(capacity: Math.Max(0, s.Length - 1));
    for (var i = 0; i < s.Length - 1; i++)
    {
        var pair = string.Create(2, s, (span, state) => { span[0] = state[i]; span[1] = state[i + 1]; });
        if (lastIndex.TryGetValue(pair, out var prev))
        {
            if (i - prev >= 2)
            {
                hasPairTwice = true;
                break;
            }
        }
        else
        {
            lastIndex[pair] = i;
        }
    }

    if (!hasPairTwice)
    {
        return false;
    }

    // Rule 2: repeats with one letter between (ABA)
    for (var i = 0; i < s.Length - 2; i++)
    {
        if (s[i] == s[i + 2])
        {
            return true;
        }
    }

    return false;
}
