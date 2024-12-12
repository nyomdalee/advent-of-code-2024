using Utils;
using Utils.Grid;

namespace Twelve;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[]>(Run);
    }

    private static readonly Dictionary<DirectionName, Direction> turnRight = new()
    {
        { DirectionName.North, Grid.CardinalDirections.First(x => x.Name == DirectionName.East) },
        { DirectionName.East, Grid.CardinalDirections.First(x => x.Name == DirectionName.South) },
        { DirectionName.South, Grid.CardinalDirections.First(x => x.Name == DirectionName.West) },
        { DirectionName.West, Grid.CardinalDirections.First(x => x.Name == DirectionName.North) },
    };

    private static long Run(string[] lines)
    {
        var grid = Grid.FromLines(lines);

        var resolved = new bool[grid.UpperBound.X + 1, grid.UpperBound.Y + 1];

        return grid.LoopWithIndices(GetGroupValue).Where(x => x != 0).Sum();

        long GetGroupValue(int x, int y, char value)
        {
            if (resolved[x, y])
            {
                return 0;
            }

            List<Point> currentSet = [new(x, y)];
            List<Point> nextSet = [];

            List<Point> group = [.. currentSet];
            do
            {
                foreach (var point in currentSet)
                {
                    foreach (var dir in Grid.CardinalDirections)
                    {
                        if (!grid.IsOutOfBounds(point, dir) && Equals(grid.GetValueAfterMove(point, dir), value))
                        {
                            var nextPoint = Grid.Step(point, dir);
                            if (!group.Contains(nextPoint) && !nextSet.Contains(nextPoint))
                            {
                                resolved[nextPoint.X, nextPoint.Y] = true;
                                nextSet.Add(nextPoint);
                            }
                        }
                    }
                }

                group.AddRange(nextSet);
                currentSet = [.. nextSet];
                nextSet = [];
            }
            while (currentSet.Count > 0);

            return group.Count * CountCorners(grid, group, value);
        }
    }

    private static long CountCorners(Grid grid, List<Point> group, char value) =>
        group.Sum(point =>
            Grid.CardinalDirections.Count(direction =>
                IsOuter(grid, point, direction, value) || IsInner(grid, point, direction, value)));


    private static bool IsOuter(Grid grid, Point point, Direction direction, char value)
    {
        var ahead = Grid.Step(point, direction);
        var rightAngle = Grid.Step(point, turnRight[direction.Name]);

        return (grid.IsOutOfBounds(ahead) || !Equals(grid.GetValue(ahead), value))
            && (grid.IsOutOfBounds(rightAngle) || !Equals(grid.GetValue(rightAngle), value));
    }

    private static bool IsInner(Grid grid, Point point, Direction direction, char value)
    {
        var ahead = Grid.Step(point, direction);
        var rightAngle = Grid.Step(point, turnRight[direction.Name]);
        var diagonal = GetDiagonal(point, ahead, rightAngle);

        if (grid.IsOutOfBounds(ahead) || grid.IsOutOfBounds(rightAngle))
        {
            return false;
        }

        return Equals(grid.GetValue(ahead), value)
            && Equals(grid.GetValue(rightAngle), value)
            && !Equals(grid.GetValue(diagonal), value);
    }

    private static Point GetDiagonal(Point pivot, Point point1, Point point2)
    {
        int x = point1.X == pivot.X ? point2.X : point1.X;
        int y = point1.Y == pivot.Y ? point2.Y : point1.Y;
        return new Point(x, y);
    }
}