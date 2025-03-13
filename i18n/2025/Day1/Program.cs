using System.Text;

List<string> inputLines = [.. File.ReadAllLines("input.txt")];

int byteCountLimit = 160; // SMS
int charCountLimit = 140; // Tweet

List<int> cents = [];

foreach (var input in inputLines)
{
    // Byte count
    int byteCount = Encoding.UTF8.GetByteCount(input);

    // Character count
    int charCount = input.Length;

    int cent = 0;

    if (byteCount <= byteCountLimit && charCount <= charCountLimit)
    {
        // Send for both SMS and Tweet
        cent = 13;
    }
    else if (byteCount <= byteCountLimit)
    {
        // Send as SMS
        cent = 11;
    }
    else if (charCount <= charCountLimit)
    {
        // Send as Tweet
        cent = 7;
    }

    cents.Add(cent);
}

// Calculate total cost
int totalCost = cents.Sum();

// Show formula
var output = "";
foreach (var cent in cents)
{
    output += cent + " + ";
}

output = output[..^3];
output += " = " + totalCost;

Console.WriteLine("Formula: " + output);
Console.ReadLine();