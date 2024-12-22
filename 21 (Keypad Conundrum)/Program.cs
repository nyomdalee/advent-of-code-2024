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

    private static readonly Dictionary<DirectionName, char> directionTranslate = new()
    {
        {DirectionName.North, '^' },
        {DirectionName.East, '>' },
        {DirectionName.South, 'v' },
        {DirectionName.West, '<' },
    };


    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    private static long Run(string[] lines)
    {
        var keypadGrid = Grid.FromLines(keypadlines);
        var arrowGrid = Grid.FromLines(arrowsLines);

        return lines.Sum(x => GetCodeComplexity(keypadGrid, arrowGrid, x));
    }

    private static int GetCodeComplexity(Grid keypadGrid, Grid arrowGrid, string code)
    {
        var withStart = "A" + code;
        List<string> keypadCombinations = [string.Empty];
        for (var i = 1; i < withStart.Length; i++)
        {
            var keyOptions = ReachKey(keypadGrid, withStart[i - 1], withStart[i]);
            List<string> nextlevel = [];
            foreach (var option in keyOptions)
            {
                foreach (var current in keypadCombinations)
                {
                    nextlevel.Add(current + option);
                }
            }
            keypadCombinations = nextlevel;
        }

        List<string> arrow1combinations = [];
        foreach (var combination in keypadCombinations)
        {
            var yep = "A" + combination;
            List<string> currentlevel = [string.Empty];
            for (var i = 1; i < yep.Length; i++)
            {
                var keyOptions = ReachKey(arrowGrid, yep[i - 1], yep[i]);
                List<string> nextlevel = [];
                foreach (var option in keyOptions)
                {
                    foreach (var current in currentlevel)
                    {
                        nextlevel.Add(current + option);
                    }
                }
                currentlevel = nextlevel;
            }
            arrow1combinations.AddRange(currentlevel);
        }

        List<string> arrow2combinations = [];
        foreach (var combination in arrow1combinations)
        {
            var yep = "A" + combination;
            List<string> currentlevel = [string.Empty];
            for (var i = 1; i < yep.Length; i++)
            {
                var keyOptions = ReachKey(arrowGrid, yep[i - 1], yep[i]);
                List<string> nextlevel = [];
                foreach (var option in keyOptions)
                {
                    foreach (var current in currentlevel)
                    {
                        nextlevel.Add(current + option);
                    }
                }
                currentlevel = nextlevel;
            }
            arrow2combinations.AddRange(currentlevel);
        }

        var finalLen = arrow2combinations.Min(x => x.Length);
        var codeNumber = int.Parse(Regex.Match(code, @"\d+").Value);
        return finalLen * codeNumber;
    }

    private static List<string> ReachKey(Grid grid, char startChar, char endChar)
    {
        var startPoint = grid.GetPointsByPredicate(x => Equals(startChar, x)).First();
        var endPoint = grid.GetPointsByPredicate(x => Equals(endChar, x)).First();

        Queue<(Point Point, List<Direction> Path)> q = [];
        q.Enqueue((startPoint, []));

        var pathLenght = Math.Abs(startPoint.X + -endPoint.X) + Math.Abs(startPoint.Y - endPoint.Y);
        List<string> possiblePaths = [];
        while (q.Count > 0)
        {
            var next = q.Dequeue();

            if (next.Point == endPoint && next.Path.Count == pathLenght)
            {
                possiblePaths.Add(ConvertDirectionsToString(next.Path));
            }

            foreach (var direction in Grid.CardinalDirections)
            {
                var newPoint = new Point(next.Point.X + direction.X, next.Point.Y + direction.Y);

                if (!grid.IsOutOfBounds(newPoint) && !Equals(grid.GetValue(newPoint), 'N'))
                {
                    List<Direction> newPath = [.. next.Path, direction];
                    if (newPath.Count < 2 || !IsTooWiggly(newPath))
                    {
                        q.Enqueue((newPoint, newPath));
                    }
                }
            }
        }

        return possiblePaths;
    }

    private static bool IsTooWiggly(List<Direction> path)
    {
        int wiggles = 0;
        for (var i = 1; i < path.Count; i++)
        {
            if (path[i - 1].Name != path[i].Name)
            {
                wiggles++;
            }
        }
        return wiggles > 1;
    }

    private static string ConvertDirectionsToString(List<Direction> directions)
    {
        var keypresses = string.Concat(directions.Select(direction => directionTranslate[direction.Name]));
        return keypresses + "A";
    }
}