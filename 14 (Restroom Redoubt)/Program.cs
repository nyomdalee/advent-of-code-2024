using System.Text.RegularExpressions;
using Utils;
using Utils.Grid;

namespace Fourteen;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string>(Run, true);
    }
    private const int Seconds = 100;
    private const int XWidth = 101;
    private const int YWidth = 103;

    private static long Run(string line)
    {
        var matches = Regex.Matches(line, @"p\=(?<Px>\d+),(?<Py>\d+) v=(?<Vx>-?\d+),(?<Vy>-?\d+)");

        var robits = matches.Cast<Match>()
            .Select(match => new Robit(
                new Point(int.Parse(match.Groups["Px"].Value), int.Parse(match.Groups["Py"].Value)),
                new Velocity(int.Parse(match.Groups["Vx"].Value), int.Parse(match.Groups["Vy"].Value))))
            .ToList();

        var robitsAfter = robits.Select(RobitPositionAfter).ToList();

        var quads = robitsAfter.Select(GetQuadrant).ToList();

        var result = quads
            .GroupBy(x => x)
            .Where(x => x.Key != 0)
            .Aggregate(1, (current, next) => current * next.Count());
        return result;
    }

    private static void DebugPrint(List<Robit> robots)
    {
        for (int j = 0; j <= YWidth - 1; j++)
        {
            for (int i = 0; i <= XWidth - 1; i++)
            {
                var c = robots.Count(x => x.Point.X == i && x.Point.Y == j);
                var wot = c > 0 ? (char)c : '.';
                Console.Write(c);
            }
            Console.WriteLine();
        }
    }

    private static Point RobitPositionAfter(Robit robit)
    {
        int x = (robit.Point.X + (robit.Velocity.X * Seconds)) % XWidth;
        int y = (robit.Point.Y + (robit.Velocity.Y * Seconds)) % YWidth;

        if (y < 0)
        {
            y += YWidth;
        }
        if (x < 0)
        {
            x += XWidth;
        }

        return new Point(x, y);
    }

    private static long GetQuadrant(Point point)
    {
        if (point.X < XWidth / 2)
        {
            if (point.Y < YWidth / 2)
            {
                return 1;
            }
            else if (point.Y > YWidth / 2)
            {
                return 2;
            }
        }

        if (point.X > XWidth / 2)
        {
            if (point.Y < YWidth / 2)
            {
                return 3;
            }
            else if (point.Y > YWidth / 2)
            {
                return 4;
            }
        }
        return 0;
    }
}

record Robit(Point Point, Velocity Velocity);
record Velocity(int X, int Y);