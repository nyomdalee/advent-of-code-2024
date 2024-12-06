using Utils;
using Utils.Grid;

namespace Six;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[]>(Run);
    }

    private static long Run(string[]? lines)
    {
        var grid = Grid.FromLines(lines);

        var dirs = Grid.CardinalDirections;

        Dictionary<DirectionName, Direction> turnRight = new()
        {
            { DirectionName.North, Grid.CardinalDirections.First(x => x.Name == DirectionName.East) },
            { DirectionName.East, Grid.CardinalDirections.First(x => x.Name == DirectionName.South) },
            { DirectionName.South, Grid.CardinalDirections.First(x => x.Name == DirectionName.West) },
            { DirectionName.West, Grid.CardinalDirections.First(x => x.Name == DirectionName.North) },
        };

        var direction = Grid.CardinalDirections.First(x => x.Name == DirectionName.North);
        var currentPoint = grid.GetPointsByPredicate(x => string.Equals(x, '^')).First();

        while (true)
        {
            grid.SetValue(currentPoint, 'X');

            if (grid.IsOutOfBounds(currentPoint, direction))
            {
                break;
            }

            while (string.Equals(grid.GetValueAfterMove(currentPoint, direction), '#'))
            {
                direction = turnRight[direction.Name];
            };

            currentPoint = Grid.Step(currentPoint, direction);
        }

        return grid.GetPointsByPredicate(x => string.Equals(x, 'X')).Count();
    }
}