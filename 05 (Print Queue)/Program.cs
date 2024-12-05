using Utils;

namespace Five;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[]>(Run);
    }

    private static long Run(string[]? lines)
    {
        var rules = lines
            .Where(x => x.Contains('|'))
            .Select(x => x.Split('|'))
            .Select(x => (First: int.Parse(x[0]), Second: int.Parse(x[1])))
            .ToLookup(x => x.Second, x => x.First);

        var updates = lines
            .Where(x => x.Contains(','))
            .Select(x => x.Split(","))
            .Select(x => x.ToList().ConvertAll(x => int.Parse(x)).ToArray())
            .ToArray();

        return updates
            .Where(update => IsValid(update, rules))
            .Sum(update => (long)update[update.Length / 2]);
    }

    private static bool IsValid(int[] update, ILookup<int, int> rules)
    {
        for (var i = 0; i < update.Length; i++)
        {
            var current = update[i];
            foreach (var r in rules[current])
            {
                if (update.Contains(r) && Array.IndexOf(update, r) >= i)
                {
                    return false;
                }
            }
        }
        return true;
    }
}