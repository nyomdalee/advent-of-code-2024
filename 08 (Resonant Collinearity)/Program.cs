using Utils;
using Utils.Grid;

namespace Eight;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
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
        }

        return antinodes.Cast<bool>().Count(x => x);
    }

    private static void OogaBooga(Grid grid, IGrouping<char, Antenna> group, bool[,] antinodes)
    {
        var ants = group.ToArray();

        if (ants.Length <= 1)
        {
            return;
        }


        for (var i = 0; i < ants.Length; i++)
        {
            var current = ants[i];
            antinodes[current.Point.X, current.Point.Y] = true;

            for (var j = i + 1; j < ants.Length; j++)
            {
                var alter = ants[j];
                var offset = (X: current.Point.X - alter.Point.X, Y: current.Point.Y - alter.Point.Y);

                var nexp = (X: current.Point.X + offset.X, Y: current.Point.Y + offset.Y);
                var nexM = (X: current.Point.X - offset.X, Y: current.Point.Y - offset.Y);

                while (!grid.IsOutOfBounds(nexp.X, nexp.Y))
                {
                    antinodes[nexp.X, nexp.Y] = true;
                    nexp = (X: nexp.X + offset.X, Y: nexp.Y + offset.Y);

                }

                while (!grid.IsOutOfBounds(nexM.X, nexM.Y))
                {
                    antinodes[nexM.X, nexM.Y] = true;
                    nexM = (X: nexM.X - offset.X, Y: nexM.Y - offset.Y);

                }
            }
        }
    }
}

record Antenna(Point Point, char Frequency);