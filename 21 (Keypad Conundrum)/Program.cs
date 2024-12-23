using System.Text.RegularExpressions;
using Utils;
using Utils.Grid;

namespace TwentyOne;

public static class Program
{
    private static readonly string[] keypadlines =
    [
        "789",
        "456",
        "123",
        "N0A",
    ];

    private static readonly string[] arrowsLines =
    [
        "N^A",
        "<v>",
    ];

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    public static long Run(string[] lines)
    {
        Grid keypadGrid = Grid.FromLines(keypadlines);
        Grid arrowGrid = Grid.FromLines(arrowsLines);
        Grid[] allLevels = [keypadGrid, .. Enumerable.Repeat(arrowGrid, 25)];

        Dictionary<(char startChar, char endChar, int level), long> sureIsACache = [];
        long total = 0;

        foreach (string line in lines)
        {
            var codeNumber = int.Parse(Regex.Match(line, @"\d+").Value);
            total += codeNumber * Expand(allLevels, line, sureIsACache);
        }

        return total;
    }

    private static long Expand(Grid[] grids, string code, Dictionary<(char currentKey, char nextKey, int level), long> sureIsACache)
    {
        if (grids.Length == 0)
        {
            return code.Length;
        }

        var currentChar = 'A';
        long result = 0;
        foreach (var nextChar in code)
        {
            result += ReachKey(grids, currentChar, nextChar, sureIsACache);
            currentChar = nextChar;
        }

        return result;
    }

    private static long ReachKey(Grid[] grids, char startChar, char endChar, Dictionary<(char currentKey, char nextKey, int level), long> sureIsACache)
    {
        if (sureIsACache.TryGetValue((startChar, endChar, grids.Length), out var cached))
        {
            return cached;
        }

        var grid = grids[0];
        var startPoint = grid.GetPointsByPredicate(x => Equals(startChar, x)).First();
        var endPoint = grid.GetPointsByPredicate(x => Equals(endChar, x)).First();
        var panicPoint = grid.GetPointsByPredicate(x => Equals('N', x)).First();

        var hori = new string(
            startPoint.X > endPoint.X ? '<' : '>',
            Math.Abs(startPoint.X - endPoint.X));

        var vert = new string(
            startPoint.Y > endPoint.Y ? 'v' : '^',
            Math.Abs(startPoint.Y - endPoint.Y));

        var result = long.MaxValue;
        if (!(startPoint.X == panicPoint.X && endPoint.Y == panicPoint.Y))
        {
            result = Math.Min(result, Expand(grids[1..], $"{vert}{hori}A", sureIsACache));
        }
        if (!(endPoint.X == panicPoint.X && startPoint.Y == panicPoint.Y))
        {
            result = Math.Min(result, Expand(grids[1..], $"{hori}{vert}A", sureIsACache));
        }

        sureIsACache.Add((startChar, endChar, grids.Length), result);
        return result;
    }
}