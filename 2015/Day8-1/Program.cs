using System.Text;

var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

int codeTotal = 0;
int memoryTotal = 0;

foreach (var raw in await File.ReadAllLinesAsync(path))
{
    if (string.IsNullOrWhiteSpace(raw))
        continue;

    var line = raw.Trim();

    // code chars are the literal length as-is
    codeTotal += line.Length;

    // compute in-memory chars by parsing escapes
    // strip surrounding quotes
    if (line.Length >= 2 && line[0] == '"' && line[^1] == '"')
    {
        var s = line.AsSpan(1, line.Length - 2);
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (c == '\\')
            {
                if (i + 1 < s.Length && s[i + 1] == '\\')
                {
                    memoryTotal += 1; // \\ -> '\\'
                    i += 1;
                }
                else if (i + 1 < s.Length && s[i + 1] == '"')
                {
                    memoryTotal += 1; // \" -> '"'
                    i += 1;
                }
                else if (i + 3 < s.Length && s[i + 1] == 'x'
                         && IsHex(s[i + 2]) && IsHex(s[i + 3]))
                {
                    memoryTotal += 1; // \x?? -> one char
                    i += 3;
                }
                else
                {
                    // Unexpected escape, treat as one char consumed for '\\'
                    memoryTotal += 1;
                }
            }
            else
            {
                memoryTotal += 1;
            }
        }
    }
    else
    {
        // If not quoted, treat whole line as code and memory same
        memoryTotal += line.Length;
    }
}

Console.WriteLine(codeTotal - memoryTotal);

static bool IsHex(char ch)
{
    return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
}
