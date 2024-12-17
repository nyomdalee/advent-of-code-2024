using System.Text.RegularExpressions;
using Utils;

namespace Seventeen;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, string>(Run);
    }

    private static string Run(string text)
    {
        var match = Regex.Match(text, @"Register A:\s*(\d+)\s*.*?\r\nRegister B:\s*(\d+)\s*.*?\r\nRegister C:\s*(\d+)\s*.*?\r\nProgram:\s*([\d,\s]+)");

        var registerA = long.Parse(match.Groups[1].Value);
        var registerB = long.Parse(match.Groups[2].Value);
        var registerC = long.Parse(match.Groups[3].Value);
        var program = Array.ConvertAll(match.Groups[4].Value.Split(','), s => long.Parse(s.Trim()));

        long i = 0;
        List<long> outputs = [];
        while (i < program.Length)
        {
            var instruction = program[i];
            var literalOperand = program[i + 1];

            switch (instruction)
            {
                case 0:
                    registerA /= (long)Math.Pow(2, GetComboOperand(literalOperand));
                    break;
                case 1:
                    registerB ^= literalOperand;
                    break;
                case 2:
                    registerB = GetComboOperand(literalOperand) % 8;
                    break;
                case 3 when registerA == 0:
                    break;
                case 3:
                    i = registerA == 0 ? i : (long)literalOperand;
                    continue;
                case 4:
                    registerB ^= registerC;
                    break;
                case 5:
                    outputs.Add(GetComboOperand(literalOperand) % 8);
                    break;
                case 6:
                    registerB = registerA / (long)Math.Pow(2, GetComboOperand(literalOperand));
                    break;
                case 7:
                    registerC = registerA / (long)Math.Pow(2, GetComboOperand(literalOperand));
                    break;
                default:
                    throw new Exception("impossible, baka");
            }
            i += 2;
        }

        return string.Join(',', outputs);

        long GetComboOperand(long operand) => operand switch
        {
            7 => throw new Exception("illegal in p1"),
            6 => registerC,
            5 => registerB,
            4 => registerA,
            _ => operand,
        };
    }
}
