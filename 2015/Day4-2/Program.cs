var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

// Read the secret key (first line)
var secret = (await File.ReadAllTextAsync(path)).Trim();

using var md5 = System.Security.Cryptography.MD5.Create();

var number = 1;
while (true)
{
    var candidate = secret + number.ToString();
    var bytes = System.Text.Encoding.ASCII.GetBytes(candidate);
    var hash = md5.ComputeHash(bytes);

    // Convert hash to lowercase hex string
    var sb = new System.Text.StringBuilder(hash.Length * 2);
    for (var i = 0; i < hash.Length; i++)
    {
        _ = sb.Append(hash[i].ToString("x2"));
    }
    var hex = sb.ToString();

    if (hex.StartsWith("000000"))
    {
        Console.WriteLine(number);
        break;
    }

    number++;
}
