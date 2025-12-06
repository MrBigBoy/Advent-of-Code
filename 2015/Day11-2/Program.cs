internal class Program
{
    private static void Main()
    {
        string input = "hepxcrrq"; // given previous password
        // First next valid password
        string first = NextValid(input);
        // Second next valid password
        string second = NextValid(first);
        Console.WriteLine(second);
    }

    private static string NextValid(string s)
    {
        string pwd = s;
        do
        {
            pwd = Increment(pwd);
        } while (!IsValid(pwd));
        return pwd;
    }

    private static string Increment(string s)
    {
        var arr = s.ToCharArray();
        int i = arr.Length - 1;
        while (i >= 0)
        {
            char c = arr[i];
            c++;
            if (c > 'z')
            {
                arr[i] = 'a';
                i--;
                continue;
            }
            arr[i] = c;
            break;
        }
        return new string(arr);
    }

    private static bool IsValid(string s)
    {
        // Rule 2: cannot contain i, o, l
        foreach (char c in s)
        {
            if (c == 'i' || c == 'o' || c == 'l')
                return false;
        }

        // Rule 1: at least one straight of 3
        bool hasStraight = false;
        for (int i = 0; i <= s.Length - 3; i++)
        {
            if (s[i + 1] == s[i] + 1 && s[i + 2] == s[i] + 2)
            {
                hasStraight = true;
                break;
            }
        }
        if (!hasStraight) return false;

        // Rule 3: at least two different, non-overlapping pairs
        int pairs = 0;
        for (int i = 0; i < s.Length - 1; i++)
        {
            if (s[i] == s[i + 1])
            {
                pairs++;
                char pairChar = s[i];
                // skip overlapping by advancing i
                i++;
                // ensure different pairs by skipping same char sequence
                while (i < s.Length - 1 && s[i] == pairChar && s[i + 1] == pairChar)
                {
                    i++;
                }
            }
        }
        return pairs >= 2;
    }
}
