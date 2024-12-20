using System.Runtime.InteropServices;
using Utils;
using Utils.Grid;

namespace Twenty;

public static class Program
{
    const char Rock = '#';

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
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

        var endValue = lowestVisited[end.X, end.Y];
        var punktoC = pathPunktos.Count;

        var yekkers = pathPunktos
            .SelectMany(x => GetCutLenghts(grid, x, lowestVisited))
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());

        var above100 = yekkers
            .Where(x => x.Key >= 100)
            .Sum(x => x.Value);

        return above100;
    }

    private static List<int> GetCutLenghts(Grid grid, Point punkto, int[,] lowestVisited)
    {
        List<int> cuts = [];
        foreach (var dir in Grid.CardinalDirections)
        {
            if (!grid.IsOutOfBounds(punkto, dir)
                && !grid.IsOutOfBounds(punkto, dir, 2)
                && Equals(grid.GetValueAfterMove(punkto, dir), Rock)
                && !Equals(grid.GetValueAfterMove(punkto, dir, 2), Rock))
            {
                var nexpos = Grid.Step(punkto, dir, 2);
                var diff = lowestVisited[nexpos.X, nexpos.Y] - 2 - lowestVisited[punkto.X, punkto.Y];

                if (diff > 0)
                {
                    cuts.Add(diff);
                }
            }
        }

        return cuts;
    }
}
