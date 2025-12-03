var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

var niceStrings = 0;

foreach (var c in await File.ReadAllLinesAsync(path))
{
    // Early reject forbidden substrings
    if (c.Contains("ab") || c.Contains("cd") || c.Contains("pq") || c.Contains("xy"))
    {
        continue;
    }

    var vowels = 0;
    var hasDoubleLetter = false;

    for (var i = 0; i < c.Length; i++)
    {
        var ch = c[i];
        var lower = char.ToLower(ch);
        switch (lower)
        {
            case 'a':
            case 'e':
            case 'i':
            case 'o':
            case 'u':
                vowels++;
                break;
        }

        if (i > 0 && lower == char.ToLower(c[i - 1]))
        {
            hasDoubleLetter = true;
        }
    }

    if (vowels >= 3 && hasDoubleLetter)
    {
        niceStrings++;
    }
}

Console.WriteLine($"Number of nice strings: {niceStrings}");
