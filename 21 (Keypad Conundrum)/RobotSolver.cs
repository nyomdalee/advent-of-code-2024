using System.Text;
using System.Text.RegularExpressions;
using Utils.Grid;

namespace TwentyOne;
internal class RobotSolver
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

    private Grid keypadGrid = Grid.FromLines(keypadlines);
    private Grid arrowGrid = Grid.FromLines(arrowsLines);

    private Dictionary<string, string> keypadCache = [];
    private Dictionary<string, string> arrowCache = [];

    public long Run(string[] lines)
    {
        return lines.Sum(SolveCode);
    }

    private long SolveCode(string code)
    {
        var instructionParts = SolveKeypad(code);

        long finalL = 0;
        foreach (var instruction in instructionParts)
        {
            var instr = instruction;
            for (var i = 0; i < 25; i++)
            {
                instr = SolveArrows(instr);
            }
            finalL += instr.Length;
        }

        var codeNumber = int.Parse(Regex.Match(code, @"\d+").Value);
        return finalL * codeNumber;
    }

    private string SolveArrows(string code)
    {
        var extendedCode = code.StartsWith('A') ? code : "A" + code;

        var sb = new StringBuilder();
        for (var i = 1; i < extendedCode.Length; i++)
        {
            sb.Append(ReachArrowKey(extendedCode[i - 1], extendedCode[i]));
        }

        return sb.ToString();
    }

    private string ReachArrowKey(char startChar, char endChar)
    {
        if (arrowCache.TryGetValue($"{startChar}{endChar}", out var chachedValue))
        {
            return chachedValue;
        }
        var startPoint = arrowGrid.GetPointsByPredicate(x => Equals(startChar, x)).First();
        var endPoint = arrowGrid.GetPointsByPredicate(x => Equals(endChar, x)).First();

        bool panic = (startPoint.X == 0 && endPoint.Y == 1) || (startPoint.Y == 1 && endPoint.X == 0);
        bool RToL = startPoint.X >= endPoint.X;

        var hori = new string(
            startPoint.X > endPoint.X ? '<' : '>',
            Math.Abs(startPoint.X - endPoint.X));

        var vert = new string(
            startPoint.Y > endPoint.Y ? 'v' : '^',
            Math.Abs(startPoint.Y - endPoint.Y));

        var result = RToL
            ? panic
                ? vert + hori + "A"
                : hori + vert + "A"
            : panic
                ? hori + vert + "A"
                : vert + hori + "A";

        arrowCache.Add($"{startChar}{endChar}", result);
        return result;
    }

    private List<string> SolveKeypad(string code)
    {
        var extendedCode = "A" + code;

        List<string> keypadStrings = [];
        for (var i = 1; i < extendedCode.Length; i++)
        {
            keypadStrings.Add(ReachKeypadKey(extendedCode[i - 1], extendedCode[i]));
        }

        return keypadStrings;
    }

    private string ReachKeypadKey(char startChar, char endChar)
    {
        if (keypadCache.TryGetValue($"{startChar}{endChar}", out var chachedValue))
        {
            return chachedValue;
        }
        var startPoint = keypadGrid.GetPointsByPredicate(x => Equals(startChar, x)).First();
        var endPoint = keypadGrid.GetPointsByPredicate(x => Equals(endChar, x)).First();

        bool panic = startPoint.Y == 0 && endPoint.X == 0;
        bool RToL = startPoint.X >= endPoint.X;

        var hori = new string(
            startPoint.X > endPoint.X ? '<' : '>',
            Math.Abs(startPoint.X - endPoint.X));

        var vert = new string(
            startPoint.Y > endPoint.Y ? 'v' : '^',
            Math.Abs(startPoint.Y - endPoint.Y));

        var result = RToL
            ? hori + vert + "A"
            : panic
                ? hori + vert + "A"
                : vert + hori + "A";

        keypadCache.Add($"{startChar}{endChar}", result);
        return result;
    }
}
