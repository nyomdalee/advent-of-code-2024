using Utils;
using Utils.Grid;

namespace Six;

public static class Program
{
    private static readonly Dictionary<DirectionName, Direction> turnRight = new()
    {
        { DirectionName.North, Grid.CardinalDirections.First(x => x.Name == DirectionName.East) },
        { DirectionName.East, Grid.CardinalDirections.First(x => x.Name == DirectionName.South) },
        { DirectionName.South, Grid.CardinalDirections.First(x => x.Name == DirectionName.West) },
        { DirectionName.West, Grid.CardinalDirections.First(x => x.Name == DirectionName.North) },
    };

    private const char Rock = '#';

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    private static long Run(string[]? lines)
    {
        var grid = Grid.FromLines(lines);

        var direction = Grid.CardinalDirections.First(x => x.Name == DirectionName.North);
        var currentPoint = grid.GetPointsByPredicate(x => Equals(x, '^')).First();
        var startPoint = new Point(currentPoint.X, currentPoint.Y);
        var startDirection = new Direction(direction.X, direction.Y, direction.Name);

        HashSet<(Point, Direction)> visited = [];

        while (true)
        {
            if (grid.IsOutOfBounds(currentPoint, direction))
            {
                visited.Add((currentPoint, direction));
                break;
            }

            if (IsRocked(grid, currentPoint, direction))
            {
                direction = turnRight[direction.Name];
                continue;
            }

            visited.Add((currentPoint, direction));

            currentPoint = Grid.Step(currentPoint, direction);
        }

        var distinctPoints = visited.Select(x => x.Item1).Distinct().ToList();
        distinctPoints.Remove(startPoint);

        return distinctPoints.Count(x => IsLoop(grid, x, startPoint, startDirection));
    }

    private static bool IsRocked(Grid grid, Point poin, Direction direction) => Equals(grid.GetValueAfterMove(poin, direction), Rock);

    private static bool IsLoop(Grid grid, Point rockPoint, Point startPoint, Direction startDirection)
    {
        var modGrid = grid.DeepCleanCopy();
        modGrid.SetValue(rockPoint, Rock);

        var point = new Point(startPoint.X, startPoint.Y);
        var direction = new Direction(startDirection.X, startDirection.Y, startDirection.Name);

        HashSet<(Point, Direction)> visited = [];

        while (true)
        {
            if (modGrid.IsOutOfBounds(point, direction))
            {
                visited.Add((point, direction));
                return false;
            }

            var nu = (point, direction);
            if (visited.Contains(nu))
            {
                return true;
            }

            if (IsRocked(modGrid, point, direction))
            {
                direction = turnRight[direction.Name];
                continue;
            }

            visited.Add(nu);

            point = Grid.Step(point, direction);
        }
    }
}
