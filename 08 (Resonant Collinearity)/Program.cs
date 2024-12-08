using Utils;
using Utils.Grid;

namespace Eight;

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

        var antennas = grid
            .LoopWithIndices((x, y, value) => new { x, y, value })
            .Where(item => !Equals(item.value, '.'))
            .Select(item => new Antenna(new Point(item.x, item.y), item.value))
            .GroupBy(x => x.Frequency)
            .ToList();

        var distinctFrequencies = antennas.ConvertAll(x => x.Key);

        bool[,] antinodes = new bool[grid.UpperBound.X + 1, grid.UpperBound.Y + 1];

        foreach (var group in antennas)
        {
            OogaBooga(grid, group, antinodes);
            Console.WriteLine($"{group.Key}:{antinodes.Cast<bool>().Count(x => x)}");

        }

        return antinodes.Cast<bool>().Count(x => x);
    }

    private static void OogaBooga(Grid grid, IGrouping<char, Antenna> group, bool[,] antinodes)
    {
        var freq = group.Key;
        var ants = group.ToArray();

        for (var i = 0; i < ants.Length; i++)
        {
            var current = ants[i];

            for (var j = i + 1; j < ants.Length; j++)
            {
                var next = ants[j];

                var offset1 = (X: current.Point.X - next.Point.X, Y: current.Point.Y - next.Point.Y);
                var offset2 = (X: next.Point.X - current.Point.X, Y: next.Point.Y - current.Point.Y);

                var cur1 = (X: current.Point.X + offset1.X, Y: current.Point.Y + offset1.Y);
                var cur2 = (X: current.Point.X + offset2.X, Y: current.Point.Y + offset2.Y);
                var nex1 = (X: next.Point.X + offset1.X, Y: next.Point.Y + offset1.Y);
                var nex2 = (X: next.Point.X + offset2.X, Y: next.Point.Y + offset2.Y);

                if (!grid.IsOutOfBounds(nex1.X, nex1.Y) && !Equals(grid.GetValue(nex1.X, nex1.Y), freq))
                {
                    antinodes[nex1.X, nex1.Y] = true;
                }
                if (!grid.IsOutOfBounds(nex2.X, nex2.Y) && !Equals(grid.GetValue(nex2.X, nex2.Y), freq))
                {
                    antinodes[nex2.X, nex2.Y] = true;
                }
                if (!grid.IsOutOfBounds(cur1.X, cur1.Y) && !Equals(grid.GetValue(cur1.X, cur1.Y), freq))
                {
                    antinodes[cur1.X, cur1.Y] = true;
                }
                if (!grid.IsOutOfBounds(cur2.X, cur2.Y) && !Equals(grid.GetValue(cur2.X, cur2.Y), freq))
                {
                    antinodes[cur2.X, cur2.Y] = true;
                }
            }
        }
    }
}

record Antenna(Point Point, char Frequency);