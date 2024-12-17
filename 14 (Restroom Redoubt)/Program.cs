using System.Text.RegularExpressions;
using Utils;

namespace Fourteen;

public static class Program
{
    private const int XWidth = 101;
    private const int YWidth = 103;

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run, true);
    }

    private static long Run(string line)
    {
        var matches = Regex.Matches(line, @"p\=(?<Px>\d+),(?<Py>\d+) v=(?<Vx>-?\d+),(?<Vy>-?\d+)");

        var robits = matches.Cast<Match>()
            .Select(match => new Robit(
                int.Parse(match.Groups["Px"].Value), int.Parse(match.Groups["Py"].Value),
                int.Parse(match.Groups["Vx"].Value), int.Parse(match.Groups["Vy"].Value)))
            .ToList();

        long lowestDanger = long.MaxValue;
        int ay = 0;
        for (int i = 1; i <= XWidth * YWidth; i++)
        {
            robits = robits.ConvertAll(RobitPositionAfter);
            var quads = robits.ConvertAll(GetQuadrant);
            var danger = quads
                .Where(x => x != 0)
                .GroupBy(x => x)
                .Aggregate(1, (current, next) => current * next.Count());

            if (danger < lowestDanger)
            {
                lowestDanger = danger;
                ay = i;
            }
        }

        return ay;
    }

    private static Robit RobitPositionAfter(Robit robit)
    {
        int x = (robit.Px + robit.Vx) % XWidth;
        int y = (robit.Py + robit.Vy) % YWidth;

        if (y < 0)
        {
            y += YWidth;
        }
        if (x < 0)
        {
            x += XWidth;
        }

        return robit with { Px = x, Py = y };
    }

    private static long GetQuadrant(Robit robit)
    {
        if (robit.Px < XWidth / 2)
        {
            if (robit.Py < YWidth / 2)
            {
                return 1;
            }
            else if (robit.Py > YWidth / 2)
            {
                return 2;
            }
        }

        if (robit.Px > XWidth / 2)
        {
            if (robit.Py < YWidth / 2)
            {
                return 3;
            }
            else if (robit.Py > YWidth / 2)
            {
                return 4;
            }
        }
        return 0;
    }
}
record Robit(int Px, int Py, int Vx, int Vy);
