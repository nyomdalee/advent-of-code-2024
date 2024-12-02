using Utils;

namespace Two;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve(Run);
    }

    private static long Run(string[] lines)
    {
        List<List<long>> diffs = [];
        foreach (var line in lines)
        {
            long[] split = Array.ConvertAll(line.Split(' '), long.Parse);

            List<long> diffPart = [];
            for (int i = 1; i < split.Length; i++)
            {
                diffPart.Add(split[i - 1] - split[i]);
            }
            diffs.Add(diffPart);
        }

        long safeCount = 0;
        foreach (var line in diffs)
        {
            if (line.All(x => x > 0 && x <= 3) || line.All(x => x < 0 && x >= -3))
            {
                safeCount++;
            }
        }

        return safeCount;
    }
}