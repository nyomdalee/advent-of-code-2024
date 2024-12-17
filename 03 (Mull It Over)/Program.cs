using System.Text.RegularExpressions;
using Utils;

namespace Three;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run);
    }

    private static long Run(string? text)
    {
        var relevant = Regex.Matches(text ?? throw new Exception("null input"), @"(?:^|do\(\)).*?(?:don't\(\)|$)", RegexOptions.Multiline);
        var matches = relevant.SelectMany(x => Regex.Matches(x.Value, @"mul\((\d{1,3}),(\d{1,3})\)"));
        return matches.Aggregate(0L, (current, match) => current + (long.Parse(match.Groups[1].Value) * long.Parse(match.Groups[2].Value)));
    }
}