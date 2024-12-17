using Utils;
using Utils.Grid;

namespace Ten;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    private static long Run(string[]? lines)
    {
        var grid = IntGrid.FromLines(lines);

        var zeroPositions = grid.GetPointsByPredicate(x => x == 0).ToList();

        int sum = 0;
        foreach (var zeroPosition in zeroPositions)
        {
            sum += EvaluatePosition(grid, zeroPosition);
        }

        return sum;
    }

    private static int EvaluatePosition(IntGrid grid, Point position)
    {
        var currentLevel = new List<Point> { position };
        var dirs = IntGrid.CardinalDirections;

        for (var i = 0; i < 9; i++)
        {
            var nextLevel = new List<Point>();
            foreach (var point in currentLevel)
            {
                foreach (var direction in dirs)
                {
                    if (!grid.IsOutOfBounds(point, direction) && grid.GetValueAfterMove(point, direction) == i + 1)
                    {
                        nextLevel.Add(IntGrid.Step(point, direction));
                    }
                }
            }
            currentLevel = nextLevel;
        }

        return currentLevel.Count;
    }
}
