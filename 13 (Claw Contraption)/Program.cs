using System.Text.RegularExpressions;
using Utils;
using Utils.Grid;

namespace Thirteen;

public static class Program
{
    private const int ACost = 3;
    private const int BCost = 1;

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string>(Run);
    }


    private static long Run(string line)
    {
        var matches = Regex.Matches(line, @"Button A: X\+(?<AX>\d+), Y\+(?<AY>\d+)\r\nButton B: X\+(?<BX>\d+), Y\+(?<BY>\d+)\r\nPrize: X\=(?<PX>\d+), Y\=(?<PY>\d+)");

        var machines = matches.Cast<Match>()
            .Select(match => new Machine(
                new Step(int.Parse(match.Groups["AX"].Value), int.Parse(match.Groups["AY"].Value)),
                new Step(int.Parse(match.Groups["BX"].Value), int.Parse(match.Groups["BY"].Value)),
                new Point(int.Parse(match.Groups["PX"].Value), int.Parse(match.Groups["PY"].Value))))
            .ToList();

        return machines.Sum(DoDumb);
    }

    private static long DoDumb(Machine machine)
    {
        int a = 100;
        int b = 0;
        var Ooga = new Point(machine.ButtonA.X * a, machine.ButtonA.Y * a);

        while ((Ooga.X != machine.Prize.X || Ooga.Y != machine.Prize.Y) && a > 0 && b < 100)
        {
            if (Ooga.X > machine.Prize.X || Ooga.Y > machine.Prize.Y)
            {
                Ooga = SubtractButton(Ooga, machine.ButtonA);
                a--;
            }

            if (Ooga.X < machine.Prize.X || Ooga.Y < machine.Prize.Y)
            {
                Ooga = AddButton(Ooga, machine.ButtonB);
                b++;
            }
        }

        return (Ooga.X == machine.Prize.X && Ooga.Y == machine.Prize.Y) ? (a * ACost) + (b * BCost) : 0;
    }

    private static Point AddButton(Point point, Step button) => new(point.X + button.X, point.Y + button.Y);
    private static Point SubtractButton(Point point, Step button) => new(point.X - button.X, point.Y - button.Y);

}

record Machine(Step ButtonA, Step ButtonB, Point Prize);
record Step(int X, int Y);