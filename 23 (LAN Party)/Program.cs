using Utils;

namespace TwentyThree;

public static class Program
{

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    public static long Run(string[] lines)
    {
        var cons = lines.Select(x =>
        {
            var split = x.Split('-').Order().ToArray();
            return new Connection(split[0], split[1]);
        }).ToList();

        var lookup = cons
            .GroupBy(x => x.First)
            .OrderBy(x => x.Key)
            .ToLookup(x => x.Key, x => x.Select(x => x.Second));

        List<Connection> check = [];
        foreach (var node in lookup)
        {
            var pairs = GeneratePairs(node.SelectMany(x => x), node.Key);
            check.AddRange(pairs);
        }

        return check.Count(x => cons.Contains(x));
    }

    static List<Connection> GeneratePairs(IEnumerable<string> list, string start)
    {
        var array = list.Order().ToArray();
        List<Connection> pairs = [];

        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                if (start.StartsWith('t') || array[i].StartsWith('t') || array[j].StartsWith('t'))
                {
                    pairs.Add(new(array[i], array[j]));
                }
            }
        }

        return pairs;
    }

    record Connection(string First, string Second);
}