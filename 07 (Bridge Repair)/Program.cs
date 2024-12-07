using Utils;

namespace Seven;

public static class Program
{
    private enum Operators
    {
        Add,
        Multi,
        Concat
    };

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[]>(Run);
    }

    private static long Run(string[]? lines)
    {
        var equations = lines.Select(x => new Equation(
            long.Parse(x.Split(':')[0]),
            x.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray()
        ));

        var longest = equations.Max(x => x.Values.Length);

        List<Operators[]> allCombinations = [];
        List<Operators[]> currentLevel =
        [
            [Operators.Add] ,
            [Operators.Multi],
            [Operators.Concat]
        ];

        allCombinations.AddRange(currentLevel);

        for (int i = 0; i < longest - 2; i++)
        {
            List<Operators[]> nextLevel = [];

            foreach (var combination in currentLevel)
            {
                nextLevel.Add([.. combination, Operators.Add]);
                nextLevel.Add([.. combination, Operators.Multi]);
                nextLevel.Add([.. combination, Operators.Concat]);
            }

            allCombinations.AddRange(nextLevel);
            currentLevel = nextLevel;
        }

        return equations
            .Where(x => IsValid(x, allCombinations))
            .Sum(x => x.Result);

    }

    private static bool IsValid(Equation equation, List<Operators[]> allCombinations)
    {
        var relevantCombinations = allCombinations.Where(x => x.Length == equation.Values.Length - 1).ToList();
        foreach (var combination in relevantCombinations)
        {
            long sum = equation.Values[0];
            for (int i = 0; i < equation.Values.Length - 1; i++)
            {
                if (combination[i] == Operators.Add)
                {
                    sum += equation.Values[i + 1];
                }
                else if (combination[i] == Operators.Multi)
                {
                    sum *= equation.Values[i + 1];
                }
                else
                {
                    sum = long.Parse($"{sum}{equation.Values[i + 1]}");
                }
            }
            if (sum == equation.Result)
            {
                return true;
            }
        }
        return false;
    }
}

record Equation(long Result, long[] Values);
