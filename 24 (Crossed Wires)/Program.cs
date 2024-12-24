using System.Text.RegularExpressions;
using Utils;

namespace TwentyThree;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run);
    }

    public static long Run(string text)
    {
        var gateMatches = Regex.Matches(text, @"(?<inOne>\w+) (?<type>\w+) (?<ineTwo>\w+) -> (?<out>\w+)");
        var allWires = gateMatches
            .SelectMany(x => new[] { x.Groups[1].Value, x.Groups[3].Value, x.Groups[4].Value })
            .Distinct()
            .Order()
            .ToDictionary(x => x, _ => (bool?)null);

        var startMatches = Regex.Matches(text, @"(?<gate>\w+): (?<value>\d)");
        foreach (Match match in startMatches)
        {
            allWires[match.Groups[1].Value] = Convert.ToBoolean(int.Parse(match.Groups[2].Value));
        }

        var gates = gateMatches
            .Select(x => new Gate(x.Groups[1].Value, x.Groups[3].Value, x.Groups[2].Value, x.Groups[4].Value))
            .ToList();

        var zWires = allWires.Keys.Where(x => x.StartsWith('z')).ToList();


        while (!ZsDone(allWires, zWires))
        {
            foreach (var gate in gates.Where(x => allWires[x.outGate] is null))
            {
                var oneValue = allWires[gate.wire1];
                var twoValue = allWires[gate.wire2];
                if (oneValue is not null && twoValue is not null)
                {
                    DoOp((bool)oneValue, (bool)twoValue, gate.outGate, gate.type, allWires);
                }
            }
        }

        return Convert.ToInt64(string.Join("", allWires
            .Where(x => zWires.Contains(x.Key)).Reverse()
            .Select(x => Convert.ToInt32(x.Value))), 2);
    }

    private static void DoOp(bool oneValue, bool twoValue, string outWire, string op, Dictionary<string, bool?> allwires)
    {
        switch (op)
        {
            case "OR":
                allwires[outWire] = oneValue || twoValue;
                break;
            case "AND":
                allwires[outWire] = oneValue && twoValue;
                break;
            case "XOR":
                allwires[outWire] = ((oneValue ? 1 : 0) ^ (twoValue ? 1 : 0)) == 1;
                break;
        }
    }

    private static bool ZsDone(Dictionary<string, bool?> allwires, List<string> zWires) =>
        zWires.All(x => allwires[x] is not null);

    record Gate(string wire1, string wire2, string type, string outGate);
}