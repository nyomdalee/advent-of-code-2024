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
        var numberLines = lines.Select(x => Array.ConvertAll(x.Split(' '), long.Parse)).ToList();

        long safeCount = 0;

        foreach (var line in numberLines)
        {
            if (BruteForce(line))
            {
                safeCount++;
            }
        }
        return safeCount;
    }
    private static bool BruteForce(long[] line)
    {
        List<long[]> exhaustiveLines = [];
        exhaustiveLines.Add(line);
        for (var i = 0; i < line.Length; i++)
        {
            long[] lineCopy = new long[line.Length];
            line.CopyTo(lineCopy, 0);
            var listLine = lineCopy.ToList();
            listLine.RemoveAt(i);
            exhaustiveLines.Add(listLine.ToArray());
        }

        foreach (var exhaustiveLine in exhaustiveLines)
        {
            List<long> diffs = [];
            for (int i = 1; i < exhaustiveLine.Length; i++)
            {
                diffs.Add(exhaustiveLine[i - 1] - exhaustiveLine[i]);
            }

            if (diffs.All(x => x > 0 && x <= 3) || diffs.All(x => x < 0 && x >= -3))
            {
                return true;
            }
        }

        return false;
    }
}