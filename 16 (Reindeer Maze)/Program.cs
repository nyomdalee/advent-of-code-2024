using Utils;
using Utils.Grid;

namespace Sixteen;

public static class Program
{
    private const char Wall = '#';
    private const char End = 'E';

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run, true);
    }

    private static long Run(string[]? lines)
    {
        var grid = Grid.FromLines(lines);

        var start = grid.GetPointsByPredicate(x => Equals(x, 'S')).First();
        var end = grid.GetPointsByPredicate(x => Equals(x, 'E')).First();
        var startDirection = Grid.GetDirectionByName(DirectionName.East);

        long[,,] lowestVisited = new long[grid.UpperBound.X + 1, grid.UpperBound.Y + 1, 4];
        var startT = (start, startDirection, 0);
        Queue<(Point Point, Direction Direction, long Cost)> toExpand = new([startT]);
        long lowestEnd = long.MaxValue;
        lowestVisited[start.X, start.Y, (int)startDirection.Name] = 0;
        while (toExpand.Count > 0)
        {
            var next = toExpand.Dequeue();

            TryExpand(grid, next, next.Direction, next.Cost + 1, lowestVisited, toExpand, ref lowestEnd);
            TryExpand(grid, next, Grid.TurnRight[next.Direction.Name], next.Cost + 1001, lowestVisited, toExpand, ref lowestEnd);
            TryExpand(grid, next, Grid.TurnLeft[next.Direction.Name], next.Cost + 1001, lowestVisited, toExpand, ref lowestEnd);
        }

        List<(Point Point, Direction Direction, long Cost)> ends = [];
        foreach (var dir in Grid.CardinalDirections)
        {
            if (lowestVisited[end.X, end.Y, (int)dir.Name] == lowestEnd)
            {
                ends.Add((new(end.X, end.Y), dir, lowestVisited[end.X, end.Y, (int)dir.Name]));
            }
        }

        List<Point> pois = [];
        foreach (var validEnd in ends)
        {
            pois = pois.Union(FindPath(validEnd, lowestVisited)).ToList();
        }

        return pois.Count + 1;
    }

    private static List<Point> FindPath((Point Point, Direction Direction, long Cost) end, long[,,] lowestVisited)
    {
        List<Point> pathPoints = [];

        List<List<(Point Point, Direction Direction, long Cost)>> currentLayer = [[end]];

        while (currentLayer.Count > 0)
        {
            List<List<(Point Point, Direction Direction, long Cost)>> nextLayer = [];
            foreach (var path in currentLayer)
            {
                var next = path.Last();

                var pointToCheck = Grid.Step(next.Point, next.Direction, -1);
                var checkDirect = next.Direction;
                var checkleft = Grid.TurnLeft[next.Direction.Name];
                var checkright = Grid.TurnRight[next.Direction.Name];

                foreach (var dir in (List<(Direction Direction, long Decrement)>)([(checkDirect, -1), (checkleft, -1001), (checkright, -1001)]))
                {
                    var dirValue = lowestVisited[pointToCheck.X, pointToCheck.Y, (int)dir.Direction.Name];
                    var expectedValue = next.Cost + dir.Decrement;
                    if (lowestVisited[pointToCheck.X, pointToCheck.Y, (int)dir.Direction.Name] == next.Cost + dir.Decrement)
                    {
                        if (next.Cost + dir.Decrement == 0)
                        {
                            pathPoints.AddRange(path.ConvertAll(x => x.Point));
                        }
                        else
                        {
                            nextLayer.Add([.. path, (pointToCheck, dir.Direction, next.Cost + dir.Decrement)]);
                        }
                    }
                }
            }
            currentLayer = nextLayer;
        }
        return pathPoints;
    }

    private static void TryExpand(Grid grid, (Point Point, Direction Direction, long Cost) next, Direction direction, long newCost, long[,,] lowestVisited, Queue<(Point, Direction, long)> toExpand, ref long lowestEnd)
    {
        if (newCost > lowestEnd)
        {
            return;
        }

        var pointToEval = Grid.Step(next.Point, direction);
        var newValue = grid.GetValue(pointToEval);

        if (Equals(newValue, End))
        {
            if (lowestVisited[pointToEval.X, pointToEval.Y, (int)direction.Name] == 0 || lowestVisited[pointToEval.X, pointToEval.Y, (int)direction.Name] > newCost)
            {
                lowestVisited[pointToEval.X, pointToEval.Y + direction.Y, (int)direction.Name] = newCost;
            }

            if (newCost < lowestEnd)
            {
                lowestEnd = newCost;
            }
            return;
        }

        if (!Equals(newValue, Wall))
        {
            if (lowestVisited[pointToEval.X, pointToEval.Y, (int)direction.Name] == 0 || lowestVisited[pointToEval.X, pointToEval.Y, (int)direction.Name] > newCost)
            {
                lowestVisited[pointToEval.X, pointToEval.Y, (int)direction.Name] = newCost;
                toExpand.Enqueue((pointToEval, direction, newCost));
            }
        }
    }
}
