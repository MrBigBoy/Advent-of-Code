namespace Day10_1;

using System.Text.RegularExpressions;

public static class Program
{
    private sealed class Machine
    {
        public int LightsCount;
        public List<int[]> Buttons = new(); // each button toggles indices
        public int[] Target; // 0/1 per light
    }

    private static IEnumerable<Machine> ParseMachines(string path)
    {
        var reLights = new Regex("\\[(?<pat>[.#]+)\\]");
        var reButtons = new Regex("\\((?<btn>[0-9,]+)\\)");
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Trim();
            if (s.Length == 0) continue;
            var mLights = reLights.Match(s);
            if (!mLights.Success) continue;
            var pat = mLights.Groups["pat"].Value;
            var machine = new Machine { LightsCount = pat.Length, Target = pat.Select(c => c == '#' ? 1 : 0).ToArray() };
            foreach (Match mb in reButtons.Matches(s))
            {
                var nums = mb.Groups["btn"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(int.Parse).ToArray();
                machine.Buttons.Add(nums);
            }
            yield return machine;
        }
    }

    private sealed class GF2Solver
    {
        // Solve A(m x n) * x = b over GF(2). Return minimal weight solution and nullspace basis.
        public static int SolveMinPresses(int m, int n, byte[][] A, byte[] b)
        {
            // Augment matrix [A|b]
            int rows = m, cols = n;
            var M = new byte[rows][];
            for (int i = 0; i < rows; i++)
            {
                M[i] = new byte[cols + 1];
                for (int j = 0; j < cols; j++) M[i][j] = A[i][j];
                M[i][cols] = b[i];
            }
            var pivotRow = new int[cols];
            for (int j = 0; j < cols; j++) pivotRow[j] = -1;
            int r = 0;
            for (int c = 0; c < cols && r < rows; c++)
            {
                int sel = -1;
                for (int i = r; i < rows; i++) if (M[i][c] == 1) { sel = i; break; }
                if (sel == -1) continue;
                // swap
                if (sel != r) { var tmp = M[sel]; M[sel] = M[r]; M[r] = tmp; }
                pivotRow[c] = r;
                // eliminate other rows
                for (int i = 0; i < rows; i++)
                {
                    if (i == r) continue;
                    if (M[i][c] == 1)
                    {
                        for (int k = c; k <= cols; k++) M[i][k] ^= M[r][k];
                    }
                }
                r++;
            }
            // Check consistency: row with all-zero A and b=1 => no solution; treat as impossible
            for (int i = 0; i < rows; i++)
            {
                bool allZero = true;
                for (int j = 0; j < cols; j++) if (M[i][j] != 0) { allZero = false; break; }
                if (allZero && M[i][cols] == 1) return int.MaxValue;
            }
            // Particular solution with free vars 0
            var x0 = new byte[cols];
            for (int c = 0; c < cols; c++)
            {
                int pr = pivotRow[c];
                if (pr != -1)
                {
                    x0[c] = M[pr][cols];
                }
                else x0[c] = 0;
            }
            var freeCols = new List<int>();
            for (int c = 0; c < cols; c++) if (pivotRow[c] == -1) freeCols.Add(c);
            int kFree = freeCols.Count;
            // Build nullspace basis vectors v for each free column f: set x_f=1, other free 0, solve pivot vars accordingly
            var basis = new List<byte[]>();
            foreach (var fcol in freeCols)
            {
                var v = new byte[cols];
                v[fcol] = 1;
                // For each pivot column c, value equals M[pivotRow[c]][cols] when RHS is 0 except contributions from free vars present in row
                // Since matrix is RREF-like with elimination on all rows, each pivot row has 1 at pivot column and some bits in higher columns. Compute v[c] as those bits at free columns fcol.
                for (int c = 0; c < cols; c++)
                {
                    int pr = pivotRow[c];
                    if (pr != -1)
                    {
                        // value equals sum of row entries at free columns times free var
                        v[c] = M[pr][fcol];
                    }
                }
                basis.Add(v);
            }
            int baseWeight = 0; for (int i = 0; i < cols; i++) baseWeight += x0[i];
            int best = baseWeight;
            if (kFree <= 20)
            {
                // Enumerate all combinations
                int combos = 1 << kFree;
                for (int mask = 1; mask < combos; mask++)
                {
                    int w = baseWeight;
                    // Compute weight delta by XORing basis vectors when bit set
                    // We can compute final x weight by tracking parity per index
                    // Keep an array parity initialized 0, update and then compute
                    // For efficiency, accumulate directly
                    var parity = new byte[cols];
                    for (int bi = 0; bi < kFree; bi++)
                    {
                        if (((mask >> bi) & 1) != 0)
                        {
                            var bv = basis[bi];
                            for (int i = 0; i < cols; i++) parity[i] ^= bv[i];
                        }
                    }
                    // weight = popcount(x0 XOR parity)
                    w = 0;
                    for (int i = 0; i < cols; i++) w += (x0[i] ^ parity[i]);
                    if (w < best) best = w;
                }
            }
            // if too many free vars, keep x0 as heuristic minimal
            return best;
        }
    }

    private static int MinPressesForMachine(Machine m)
    {
        int mLights = m.LightsCount;
        int nButtons = m.Buttons.Count;
        // Build A: mLights x nButtons
        var A = new byte[mLights][];
        for (int i = 0; i < mLights; i++) A[i] = new byte[nButtons];
        for (int j = 0; j < nButtons; j++)
        {
            foreach (var idx in m.Buttons[j])
            {
                if (idx >= 0 && idx < mLights) A[idx][j] ^= 1; // toggle
            }
        }
        var b = m.Target.Select(t => (byte)t).ToArray();
        var res = GF2Solver.SolveMinPresses(mLights, nButtons, A, b);
        if (res == int.MaxValue) return 0; // no solution, shouldn't happen per problem
        return res;
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
