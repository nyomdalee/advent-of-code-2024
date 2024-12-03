using System.Text.RegularExpressions;
using Utils;

namespace Two;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve(Run);
    }

    private static long Run(string? text)
    {
        var matches = Regex.Matches(text ?? throw new Exception("null input"), @"mul\((\d{1,3}),(\d{1,3})\)");
        var what = matches[0].Groups[1].Value;

        return matches.Aggregate(0L, (current, match) =>
            current + (long.Parse(match.Groups[1].Value) * long.Parse(match.Groups[2].Value)));
    }
}