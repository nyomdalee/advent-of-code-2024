using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Utils;
using Utils.Grid;

namespace Eighteen;

public static class Program
{
    const int XSize = 71;
    const int YSize = 71;
    const int SimulateCount = 1024;
    const char Empty = '.';
    const char Rock = '#';

    static readonly Point Start = new(0, 0);
    static readonly Point End = new(XSize - 1, YSize - 1);

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run, true);
    }

    private static long Run(string text)
    {
        var matches = Regex.Matches(text, @"(?<x>\d+),(?<y>\d+)");

        var rocks = matches.Cast<Match>()
                .Select(x => new Point(int.Parse(x.Groups["x"].Value), int.Parse(x.Groups["y"].Value))).ToList(); ;

        var grid = Grid.UniformOfSize(XSize, YSize, Empty);

        for (int i = 0; i < SimulateCount; i++)
        {
            grid.SetValue(rocks[i], Rock);
        }

        int[,] lowestVisited = new int[XSize, YSize];
        Span<int> span = MemoryMarshal.CreateSpan(ref lowestVisited[0, 0], XSize * YSize);
        span.Fill(int.MaxValue);

        List<(Point Point, int Cost)> currentLayer = [(Start, 0)];

        while (currentLayer.Count > 0)
        {
            List<(Point, int Cost)> nextLayer = [];
            foreach (var node in currentLayer)
            {

                if (lowestVisited[node.Point.X, node.Point.Y] > node.Cost)
                {
                    lowestVisited[node.Point.X, node.Point.Y] = node.Cost;
                    nextLayer.AddRange(grid.Expand(node.Point, Grid.CardinalDirections)
                        .Where(x => !Equals(grid.GetValue(x), Rock))
                        .Select(x => (x, node.Cost + 1)));
                }
            }
            currentLayer = nextLayer;
        }
        return lowestVisited[XSize - 1, YSize - 1];
    }
}
