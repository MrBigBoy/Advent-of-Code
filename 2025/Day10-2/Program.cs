namespace Day10_2;

using System.Text.RegularExpressions;

public static class Program
{
    private sealed class Machine
    {
        public int CountersCount;
        public List<int[]> Buttons = new();
        public int[] Target;
    }

    private static IEnumerable<Machine> ParseMachines(string path)
    {
        var reButtons = new Regex("\\((?<btn>[0-9,]+)\\)");
        var reTargets = new Regex("\\{(?<nums>[0-9,]+)\\}");
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            var mTargets = reTargets.Match(s);
            if (!mTargets.Success) continue;
            var nums = mTargets.Groups["nums"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse).ToArray();
            var machine = new Machine { CountersCount = nums.Length, Target = nums };
            foreach (Match mb in reButtons.Matches(s))
            {
                var btn = mb.Groups["btn"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(int.Parse).ToArray();
                machine.Buttons.Add(btn.Distinct().ToArray());
            }
            yield return machine;
        }
    }

    // Part 2 solver: recursive parity decomposition
    private static long MinPressesForMachine(Machine m)
    {
        return FindMinimumPresses(m.Target, m.Buttons);
    }

    private static bool AchievesPattern(int buttonMask, bool[] targetPattern, List<int[]> buttons)
    {
        var state = new bool[targetPattern.Length];
        for (int i = 0; i < buttons.Count; i++)
        {
            if ((buttonMask & (1 << i)) == 0) continue;
            foreach (var idx in buttons[i])
            {
                if (idx >= 0 && idx < state.Length) state[idx] = !state[idx];
            }
        }
        for (int i = 0; i < state.Length; i++) if (state[i] != targetPattern[i]) return false;
        return true;
    }

    private static long FindMinimumPresses(int[] targets, List<int[]> buttons)
    {
        var memo = new Dictionary<string, long>();
        return FindMinimumPressesRec(targets, buttons, memo);
    }

    private static long FindMinimumPressesRec(int[] targets, List<int[]> buttons, Dictionary<string, long> memo)
    {
        // Base case: all zeros
        if (targets.All(x => x == 0)) return 0;
        // Impossible negative
        if (targets.Any(x => x < 0)) return 1_000_000L;
        var key = string.Join(',', targets);
        if (memo.TryGetValue(key, out var cached)) return cached;

        // Parity pattern
        var parity = new bool[targets.Length];
        for (int i = 0; i < targets.Length; i++) parity[i] = (targets[i] & 1) == 1;

        int nButtons = buttons.Count;
        var paritySolutions = new List<int>();
        // Enumerate all button masks (GF(2))
        int limit = 1 << nButtons;
        for (int mask = 0; mask < limit; mask++)
        {
            if (AchievesPattern(mask, parity, buttons)) paritySolutions.Add(mask);
        }
        if (paritySolutions.Count == 0)
        {
            memo[key] = 1_000_000L;
            return 1_000_000L;
        }

        long minCost = long.MaxValue;
        foreach (var mask in paritySolutions)
        {
            var remaining = (int[])targets.Clone();
            int oddPresses = 0;
            for (int i = 0; i < nButtons; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    oddPresses++;
                    foreach (var idx in buttons[i])
                    {
                        if (idx >= 0 && idx < remaining.Length) remaining[idx] -= 1;
                    }
                }
            }
            // Must be non-negative even remainder
            bool invalid = false;
            for (int i = 0; i < remaining.Length; i++)
            {
                if (remaining[i] < 0 || (remaining[i] & 1) != 0) { invalid = true; break; }
            }
            if (invalid) continue;
            var halved = new int[remaining.Length];
            for (int i = 0; i < remaining.Length; i++) halved[i] = remaining[i] / 2;
            long rec = FindMinimumPressesRec(halved, buttons, memo);
            if (rec >= 1_000_000L) continue;
            long cost = oddPresses + 2L * rec;
            if (cost < minCost) minCost = cost;
        }

        if (minCost == long.MaxValue) minCost = 1_000_000L;
        memo[key] = minCost;
        return minCost;
    }

    public static void Main()
    {
        var runAsTest = false;
        var path = runAsTest
            ? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "testinput.txt"))
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        long total = 0;
        foreach (var m in ParseMachines(path))
        {
            total += MinPressesForMachine(m);
        }
        Console.WriteLine(total);
    }
}
