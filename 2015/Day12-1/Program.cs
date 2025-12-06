using System.Text.Json;

internal class Program
{
    private static void Main()
    {
        var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.json"));
        var json = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(json);
        long sum = Sum(doc.RootElement);
        Console.WriteLine(sum);
    }

    private static long Sum(JsonElement el)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Number:
                if (el.TryGetInt64(out long n)) return n;
                if (el.TryGetDouble(out double d)) return (long)d;
                return 0;
            case JsonValueKind.String:
                return 0;
            case JsonValueKind.Array:
                long total = 0;
                foreach (var item in el.EnumerateArray())
                    total += Sum(item);
                return total;
            case JsonValueKind.Object:
                long objTotal = 0;
                foreach (var prop in el.EnumerateObject())
                    objTotal += Sum(prop.Value);
                return objTotal;
            default:
                return 0;
        }
    }
}
