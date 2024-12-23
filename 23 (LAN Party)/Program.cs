using Utils;

namespace TwentyThree;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], string>(Run);
    }

    public static string Run(string[] lines)
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

        List<string> biggestSet = [];
        foreach (var node in lookup)
        {
            var subsets = GenerateSubsets([.. node.SelectMany(x => x), node.Key]);
            foreach (var set in subsets)
            {
                if (set.Count > biggestSet.Count)
                {
                    var pairs = GeneratePairs(set);
                    if (pairs.All(x => cons.Contains(x)))
                    {
                        biggestSet = set;
                    }
                }
            }
        }

        return string.Join(",", biggestSet.Order());
    }

    static List<Connection> GeneratePairs(IEnumerable<string> list)
    {
        var array = list.Order().ToArray();
        List<Connection> pairs = [];

        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                pairs.Add(new(array[i], array[j]));
            }
        }

        return pairs;
    }

    static List<List<string>> GenerateSubsets(string[] array)
    {
        List<List<string>> result = new List<List<string>>();
        GenerateSubsetsRecursive(array, 0, new List<string>(), result);
        return result;
    }

    static void GenerateSubsetsRecursive(string[] array, int index, List<string> current, List<List<string>> result)
    {
        if (current.Count > 0)
        {
            result.Add(new List<string>(current));
        }

        for (int i = index; i < array.Length; i++)
        {
            current.Add(array[i]);

            GenerateSubsetsRecursive(array, i + 1, current, result);

            current.RemoveAt(current.Count - 1);
        }
    }

    record Connection(string First, string Second);
}