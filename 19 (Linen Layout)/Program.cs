using Utils;

namespace Nineteen;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    private static long Run(string[] lines)
    {
        string[] towels = lines[0].Split(", ");
        string[] requests = lines[2..^0];

        return requests.Sum(x => GetCombinationCount(x, towels));
    }

    private static long GetCombinationCount(string request, string[] towels)
    {
        var validForPosition = ValidTowelLengths(request, towels);
        Dictionary<int, long> currentLayer = new() { { 0, 1 } };

        long combinations = 0;
        while (currentLayer.Count > 0)
        {
            Dictionary<int, long> nextLayer = [];

            foreach (var current in currentLayer)
            {
                foreach (var length in validForPosition[current.Key].ToList())
                {
                    int combinedLength = current.Key + length;

                    if (combinedLength == request.Length)
                    {
                        combinations += current.Value;
                    }
                    else
                    {
                        nextLayer.TryAddCount(combinedLength, current.Value);
                    }
                }
            }

            currentLayer = nextLayer;
        }
        return combinations;
    }

    static ILookup<int, int> ValidTowelLengths(string request, string[] towels)
    {
        List<(int i, int l)> lengths = [];
        for (int i = 0; i < request.Length; i++)
        {
            foreach (var towel in towels)
            {
                if (towel.Length + i > request.Length)
                {
                    continue;
                }

                var subs = request[i..(i + towel.Length)];
                if (Equals(subs, towel))
                {
                    lengths.Add((i, towel.Length));
                }
            }
        }
        return lengths.ToLookup(x => x.i, x => x.l);
    }

    public static void TryAddCount(this Dictionary<int, long> things, int len, long count)
    {
        if (!things.TryAdd(len, count))
        {
            things[len] += count;
        }
    }
}
