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

        long lowestDanger = long.MaxValue;
        int ay = 0;
        for (int i = 1; i <= XWidth * YWidth; i++)
        {
            robits = robits.ConvertAll(RobitPositionAfter);
            var quads = robits.ConvertAll(GetQuadrant);
            var danger = quads
                .GroupBy(x => x)
                .Where(x => x.Key != 0)
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
        int x = (robit.Point.X + robit.Velocity.X) % XWidth;
        int y = (robit.Point.Y + robit.Velocity.Y) % YWidth;

        if (y < 0)
        {
            y += YWidth;
        }
        if (x < 0)
        {
            x += XWidth;
        }

        return robit with { Point = new Point(x, y) };
    }

    private static long GetQuadrant(Robit robit)
    {
        if (robit.Point.X < XWidth / 2)
        {
            if (robit.Point.Y < YWidth / 2)
            {
                return 1;
            }
            else if (robit.Point.Y > YWidth / 2)
            {
                return 2;
            }
        }

        if (robit.Point.X > XWidth / 2)
        {
            if (robit.Point.Y < YWidth / 2)
            {
                return 3;
            }
            else if (robit.Point.Y > YWidth / 2)
            {
                return 4;
            }
        }
        return 0;
    }
}

record Robit(Point Point, Velocity Velocity);
record Velocity(int X, int Y);