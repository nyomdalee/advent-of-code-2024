using Utils;
using Utils.Grid;

namespace Fifteen;

public static class Program
{
    private const char Robot = '@';
    private const char Box = 'O';
    private const char Wall = '#';
    private const char Empty = '.';

    private static readonly Dictionary<char, Direction> dirTranslate = new()
    {
        { 'v', Grid.GetDirectionByName(DirectionName.North) },
        { '>', Grid.GetDirectionByName(DirectionName.East) },
        { '^', Grid.GetDirectionByName(DirectionName.South) },
        { '<', Grid.GetDirectionByName(DirectionName.West) },
    };

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string>(Run);
    }

    private static long Run(string lines)
    {
        var yek = lines.Split("\r\n\r\n");

        var e = yek[0].Split("\r\n").ToList();
        e.Reverse();
        var grid = Grid.FromLines(e.ToArray());

        var distr = yek[1];
        distr = distr.Replace(Environment.NewLine, "");
        List<Direction> moves = new List<Direction>();
        foreach (var chara in distr)
        {
            moves.Add(dirTranslate[chara]);
        }

        var robitPosition = grid.GetPointsByPredicate(x => Equals(x, Robot)).First();
        PlaySokoban(grid, robitPosition, moves);

        var boxes = grid.GetPointsByPredicate(x => Equals(x, Box)).ToList();
        return boxes.Aggregate(0, (current, next) => current + (next.Y * 100) + (next.X));
    }

    private static void PlaySokoban(Grid grid, Point robit, List<Direction> moves)
    {
        foreach (var move in moves)
        {
            var valueAfter = grid.GetValueAfterMove(robit, move);

            if (Equals(valueAfter, Empty))
            {
                grid.SetValue(robit, Empty);
                robit = Grid.Step(robit, move);
                grid.SetValue(robit, Robot);
            }

            if (Equals(valueAfter, Box) && TryGetPushLength(grid, robit, move, out var length))
            {
                Push(grid, robit, move, length);
                robit = Grid.Step(robit, move);
            }
        }
    }

    private static bool TryGetPushLength(Grid grid, Point robit, Direction move, out int length)
    {
        length = 1;
        while (true)
        {
            length++;
            var value = grid.GetValueAfterMove(robit, move, length);

            if (Equals(value, Wall))
            {
                return false;
            }

            if (Equals(value, Empty))
            {
                return true;
            }
        }
    }

    private static void Push(Grid grid, Point robit, Direction move, int length)
    {
        grid.SetValue(robit, Empty);
        for (var i = 1; i <= length; i++)
        {
            grid.SetValueAfterMove(robit, move, i == 1 ? Robot : Box, i);
        }
    }
}
