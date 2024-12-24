using System.Text.RegularExpressions;
using Utils;

namespace TwentyFour;

public static class Program
{
    const string xLSB = "x00";
    const string yLSB = "y00";
    const string zMSB = "z45";

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, string>(Run, true);
    }

    public static string Run(string text)
    {
        var gateMatches = Regex.Matches(text, @"(?<inOne>\w+) (?<type>\w+) (?<ineTwo>\w+) -> (?<out>\w+)");

        var gates = gateMatches
            .Select(x => new Gate(
                x.Groups[1].Value,
                x.Groups[3].Value,
                ToGateType(x.Groups[2].Value),
                x.Groups[4].Value))
            .ToDictionary(x => x.OutWire, x => x);

        return ValidateAdderStructure(gates);
    }

    private static string ValidateAdderStructure(Dictionary<string, Gate> gates)
    {
        var invalidOutputGates = gates
            .Where(x => x.Key.StartsWith('z')
                && x.Key != zMSB
                && x.Value.Type != GateType.XOR)
            .Select(x => x.Key)
            .ToHashSet();

        var xorGates = gates
            .Where(x => x.Value.Type == GateType.XOR)
            .ToList();

        var invalidInputSumGates = xorGates
            .Where(x => !x.Key.StartsWith('z')
                && !x.Value.In1.StartsWith('x') && !x.Value.In1.StartsWith('y')
                && !x.Value.In2.StartsWith('x') && !x.Value.In2.StartsWith('y'))
            .Select(x => x.Key)
            .ToHashSet();

        var invalidOutputSumGates = xorGates
            .Where(x => gates
                .Any(y => y.Value.Type == GateType.OR
                    && (x.Key == y.Value.In1 || x.Key == y.Value.In2)))
            .Select(x => x.Key)
            .ToHashSet();

        var invalidCarryGates = gates
            .Where(x => x.Value.Type == GateType.AND
                && x.Value.In1 is not (xLSB or yLSB)
                && x.Value.In2 is not (xLSB or yLSB)
                && gates.Any(y => y.Value.Type != GateType.OR
                    && (x.Key == y.Value.In1 || x.Key == y.Value.In2)))
            .Select(x => x.Key)
            .ToHashSet();

        var invalid = invalidOutputGates
            .Union(invalidInputSumGates)
            .Union(invalidCarryGates)
            .Union(invalidOutputSumGates);

        return string.Join(',', invalid.Order());
    }

    private static GateType ToGateType(string gateType) => gateType switch
    {
        "OR" => GateType.OR,
        "AND" => GateType.AND,
        "XOR" => GateType.XOR,
        _ => throw new InvalidDataException()
    };

    private record Gate(string In1, string In2, GateType Type, string OutWire);

    private enum GateType
    {
        AND,
        OR,
        XOR
    }
}