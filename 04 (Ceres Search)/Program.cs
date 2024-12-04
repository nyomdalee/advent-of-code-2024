using Utils;
using Utils.Grid;

namespace Four;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve(Run);
    }

    private static long Run(string[]? lines)
    {
        var grid = Grid.FromLines(lines);

        var XPositions = grid.GetPointsByPredicate(x => string.Equals(x, 'X')).ToList();

        var xmasCount = 0;
        foreach (var point in XPositions)
        {
            foreach (var direction in Grid.AllDirections)
            {
                if (IsValid(grid, point, direction))
                {
                    xmasCount++;
                }
            }
        }

        return xmasCount;
    }
    private static bool IsValid(Grid grid, Point point, Direction direction)
    {
        var endPoint = new Point(point.X + (direction.X * 3), point.Y + (direction.Y * 3));
        if (grid.IsOutOfBounds(endPoint))
        {
            return false;
        }

        // kekeke
        char[] xmas = ['X', 'M', 'A', 'S'];

        for (var i = 1; i < xmas.Length; i++)
        {
            var nextChar = grid.GetValue(point.X + (direction.X * i), point.Y + (direction.Y * i));
            if (!string.Equals(nextChar, xmas[i]))
            {
                return false;
            }
        }
        return true;
    }
}