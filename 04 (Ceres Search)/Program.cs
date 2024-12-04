using Utils;
using Utils.Grid;

namespace Four;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve(Run);
    }

    private static long Run(string[]? lines)
    {
        var grid = Grid.FromLines(lines);

        var APositions = grid.GetPointsByPredicate(x => string.Equals(x, 'A')).ToList();

        var directions = Grid.DiagonalDirections.ToDictionary(x => x.Name, x => x);

        var xmasCount = 0;
        foreach (var point in APositions)
        {
            if (IsValid(grid, point, directions))
            {
                xmasCount++;
            }
        }

        return xmasCount;
    }

    private static bool IsValid(Grid grid, Point point, Dictionary<DirectionName, Direction> directions)
    {
        foreach (var dir in directions.Values)
        {
            if (grid.IsOutOfBounds(point.X + dir.X, point.Y + dir.Y))
            {
                return false;
            }
        }

        // This is one of the dumbest things I have ever written.
        var northWest = grid.GetValueAfterMove(point, directions[DirectionName.NorthWest]);
        var northEast = grid.GetValueAfterMove(point, directions[DirectionName.NorthEast]);
        var southWest = grid.GetValueAfterMove(point, directions[DirectionName.SouthWest]);
        var southEast = grid.GetValueAfterMove(point, directions[DirectionName.SouthEast]);

        var pairOne = $"{northEast}{southWest}";
        var pairTwo = $"{northWest}{southEast}";

        List<string> both = [pairOne, pairTwo];

        foreach (var pair in both)
        {
            if (!string.Equals(pair, "SM") && !string.Equals(pair, "MS"))
            {
                return false;
            }
        }
        return true;
    }
}