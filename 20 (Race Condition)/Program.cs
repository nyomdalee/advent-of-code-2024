using System.Runtime.InteropServices;
using Utils;
using Utils.Grid;

namespace Twenty;

public static class Program
{
    const char Rock = '#';
    const int CheatDuration = 20;

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run, true);
    }

    private static long Run(string[] lines)
    {
        var grid = Grid.FromLines(lines);

        var start = grid.GetPointsByPredicate(x => Equals(x, 'S')).First();
        var end = grid.GetPointsByPredicate(x => Equals(x, 'E')).First();

        int[,] lowestVisited = new int[grid.UpperBound.X, grid.UpperBound.Y];
        Span<int> span = MemoryMarshal.CreateSpan(ref lowestVisited[0, 0], (grid.UpperBound.X) * (grid.UpperBound.Y));
        span.Fill(int.MaxValue);

        List<(Point Point, int Cost)> currentLayer = [(start, 0)];

        List<Point> pathPunktos = [start];

        while (currentLayer.Count > 0)
        {
            List<(Point Point, int Cost)> nextLayer = [];
            foreach (var node in currentLayer)
            {
                if (lowestVisited[node.Point.X, node.Point.Y] > node.Cost)
                {
                    lowestVisited[node.Point.X, node.Point.Y] = node.Cost;
                    nextLayer.AddRange(grid.Expand(node.Point, Grid.CardinalDirections)
                        .Where(x => !Equals(grid.GetValue(x), Rock))
                        .Where(x => lowestVisited[x.X, x.Y] > node.Cost + 1)
                        .Select(x => (x, node.Cost + 1)));
                }
            }
            pathPunktos.AddRange(nextLayer.Select(x => x.Point));
            currentLayer = nextLayer;
        }

        var offsets = GetReachableOffsetsWithSteps();
        return pathPunktos
            .SelectMany(x => GetCutLenghts(grid, x, lowestVisited, offsets))
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count())
            .Where(x => x.Key >= 100)
            .Sum(x => x.Value);
    }

    public static Dictionary<Point, int> GetReachableOffsetsWithSteps()
    {
        var start = new Point(0, 0);

        var pointsWithSteps = new Dictionary<Point, int>() { { start, 0 } };
        var queue = new Queue<(Point position, int steps)>();

        queue.Enqueue((start, 0));

        while (queue.Count > 0)
        {
            var (currentPoint, currentSteps) = queue.Dequeue();

            if (currentSteps >= CheatDuration)
            {
                continue;
            }

            foreach (var direction in Grid.CardinalDirections)
            {
                var newPoint = new Point(currentPoint.X + direction.X, currentPoint.Y + direction.Y);

                if (pointsWithSteps.TryAdd(newPoint, currentSteps + 1))
                {
                    queue.Enqueue((newPoint, currentSteps + 1));
                }
            }
        }

        return pointsWithSteps;
    }

    private static List<int> GetCutLenghts(Grid grid, Point punkto, int[,] lowestVisited, Dictionary<Point, int> offsets)
    {
        List<int> cuts = [];
        foreach (var offset in offsets)
        {
            var newPoint = new Point(punkto.X + offset.Key.X, punkto.Y + offset.Key.Y);
            if (!grid.IsOutOfBounds(newPoint) && !Equals(grid.GetValue(newPoint), Rock))
            {
                var diff = lowestVisited[newPoint.X, newPoint.Y] - offset.Value - lowestVisited[punkto.X, punkto.Y];
                if (diff > 0)
                {
                    cuts.Add(diff);
                }
            }
        }

        return cuts;
    }
}
