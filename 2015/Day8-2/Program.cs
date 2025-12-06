var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

int codeTotal = 0;
int encodedTotal = 0;

foreach (var raw in await File.ReadAllLinesAsync(path))
{
    if (string.IsNullOrWhiteSpace(raw))
        continue;

    var line = raw.Trim();

    // code chars are the literal length as-is
    codeTotal += line.Length;

    // encoded representation length: surrounding quotes plus escaping of \ and "
    int encLen = 2; // for new surrounding quotes
    for (int i = 0; i < line.Length; i++)
    {
        char ch = line[i];
        if (ch == '"' || ch == '\\')
        {
            encLen += 2; // escaped as \" or \\
        }
        else
        {
            encLen += 1;
        }
    }
    encodedTotal += encLen;
}

Console.WriteLine(encodedTotal - codeTotal);
