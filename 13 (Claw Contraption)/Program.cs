using System.Text.RegularExpressions;
using Utils;

namespace Thirteen;

public static class Program
{
    private const int ACost = 3;
    private const int BCost = 1;
    private const long PrizeOffset = 10_000_000_000_000;

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run, true);
    }

    private static long Run(string line)
    {
        var matches = Regex.Matches(line, @"Button A: X\+(?<AX>\d+), Y\+(?<AY>\d+)\r\nButton B: X\+(?<BX>\d+), Y\+(?<BY>\d+)\r\nPrize: X\=(?<PX>\d+), Y\=(?<PY>\d+)");

        var equations = matches.Cast<Match>()
            .Select(match => new Equations(
                int.Parse(match.Groups["AX"].Value), int.Parse(match.Groups["AY"].Value),
                int.Parse(match.Groups["BX"].Value), int.Parse(match.Groups["BY"].Value),
                int.Parse(match.Groups["PX"].Value) + PrizeOffset, int.Parse(match.Groups["PY"].Value) + PrizeOffset))
            .ToList();

        return equations.Sum(DoDumb);
    }

    private static long DoDumb(Equations equations)
    {
        long a = SolveForA(equations);
        long b = SolveForB(equations, a);

        return ValidateForIntegers(equations, a, b) ? (a * ACost) + (b * BCost) : 0;
    }

    private static long SolveForA(Equations eq) =>
        ((eq.Px * eq.By) - (eq.Py * eq.Bx))
        / ((eq.Ax * eq.By) - (eq.Ay * eq.Bx));
    private static long SolveForB(Equations eq, long a) =>
        (eq.Py - (a * eq.Ay)) / eq.By;

    private static bool ValidateForIntegers(Equations eq, long a, long b) =>
        (eq.Px == (eq.Ax * a) + (eq.Bx * b))
        && eq.Py == (eq.Ay * a) + (eq.By * b);
}

record Equations(long Ax, long Ay, long Bx, long By, long Px, long Py);
