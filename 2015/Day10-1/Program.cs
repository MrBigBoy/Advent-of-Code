internal class Program
{
    private static void Main()
    {
        string input = "1113222113";
        string current = input;
        for (int i = 0; i < 40; i++)
        {
            current = LookAndSay(current);
        }
        Console.WriteLine(current.Length);
    }

    private static string LookAndSay(string s)
    {
        var sb = new System.Text.StringBuilder(s.Length * 2);
        int i = 0;
        while (i < s.Length)
        {
            char c = s[i];
            int j = i + 1;
            while (j < s.Length && s[j] == c) j++;
            int count = j - i;
            sb.Append(count);
            sb.Append(c);
            i = j;
        }
        return sb.ToString();
    }
}
