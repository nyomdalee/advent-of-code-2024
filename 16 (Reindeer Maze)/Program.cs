using Utils;
using Utils.Grid;

namespace Sixteen;

public static class Program
{
    private const char Wall = '#';
    private const char End = 'E';

    private static readonly Dictionary<DirectionName, Direction> turnRight = new()
    {
        { DirectionName.North, Grid.GetDirectionByName(DirectionName.East) },
        { DirectionName.East, Grid.GetDirectionByName(DirectionName.South) },
        { DirectionName.South, Grid.GetDirectionByName(DirectionName.West) },
        { DirectionName.West, Grid.GetDirectionByName(DirectionName.North) },
    };

    private static readonly Dictionary<DirectionName, Direction> turnLeft = new()
    {
        { DirectionName.North, Grid.GetDirectionByName(DirectionName.West) },
        { DirectionName.East, Grid.GetDirectionByName(DirectionName.North) },
        { DirectionName.South, Grid.GetDirectionByName(DirectionName.East) },
        { DirectionName.West, Grid.GetDirectionByName(DirectionName.South) },
    };

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[]>(Run);
    }

    private static long Run(string[]? lines)
    {
        var grid = Grid.FromLines(lines);

        var start = grid.GetPointsByPredicate(x => Equals(x, 'S')).First();
        var end = grid.GetPointsByPredicate(x => Equals(x, 'E')).First();
        var startDirection = Grid.GetDirectionByName(DirectionName.East);

        long[,,] lowestVisited = new long[grid.UpperBound.X + 1, grid.UpperBound.Y + 1, 4];

        Queue<(Point Point, Direction Direction, long Cost)> toExpand = new([(start, startDirection, 0)]);
        long lowestEnd = long.MaxValue;

        while (toExpand.Count > 0)
        {
            var next = toExpand.Dequeue();

            TryExpand(grid, next, next.Direction, next.Cost + 1, lowestVisited, toExpand, ref lowestEnd);
            TryExpand(grid, next, turnRight[next.Direction.Name], next.Cost + 1001, lowestVisited, toExpand, ref lowestEnd);
            TryExpand(grid, next, turnLeft[next.Direction.Name], next.Cost + 1001, lowestVisited, toExpand, ref lowestEnd);
        }

        return lowestEnd;
    }

    private static void TryExpand(Grid grid, (Point Point, Direction Direction, long Cost) next, Direction direction, long newCost, long[,,] lowestVisited, Queue<(Point, Direction, long)> toExpand, ref long lowestEnd)
    {
        if (newCost >= lowestEnd)
        {
            return;
        }

        var newValue = grid.GetValueAfterMove(next.Point, direction);
        if (Equals(newValue, End))
        {
            if (newCost < lowestEnd)
            {
                lowestEnd = newCost;
            }
            return;
        }

        if (!Equals(newValue, Wall))
        {
            if (lowestVisited[next.Point.X, next.Point.Y, (int)direction.Name] == 0 || lowestVisited[next.Point.X, next.Point.Y, (int)direction.Name] > newCost)
            {
                lowestVisited[next.Point.X, next.Point.Y, (int)direction.Name] = newCost;
                toExpand.Enqueue((Grid.Step(next.Point, direction), direction, newCost));
            }
        }
    }
}
