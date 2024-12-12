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

    private static long Run(string[] lines)
    {
        var grid = Grid.FromLines(lines);

        var resolved = new bool[grid.UpperBound.X + 1, grid.UpperBound.Y + 1];

        return grid.LoopWithIndices(DoThing).Where(x => x != 0).Sum();

        long DoThing(int x, int y, char value)
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

            int perim = 0;
            foreach (var point in group)
            {
                foreach (var dir in Grid.CardinalDirections)
                {
                    if (grid.IsOutOfBounds(point, dir) || !Equals(grid.GetValueAfterMove(point, dir), value))
                    {
                        perim++;
                    }
                }
            }

            return perim * group.Count;
        }
    }
}