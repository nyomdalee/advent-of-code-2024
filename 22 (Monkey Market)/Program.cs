using Utils;

namespace TwentyTwo;

public static class Program
{
    const int iterations = 2000;
    const long modulo = 16777216;

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run);
    }

    private static long Run(string[] lines)
    {
        var secrets = lines.Select(int.Parse).ToList();

        var histories = secrets.ConvertAll(IterateSecret);
        var transformed = histories.ConvertAll(DictifyHistory);

        var sequences = transformed.SelectMany(x => x).Select(x => x.Key).Distinct().ToList();

        return sequences.ConvertAll(x => GetMaximumSequenceValue(x, transformed)).Max();
    }

    private static long GetMaximumSequenceValue(string combination, List<Dictionary<string, int>> histories)
    {
        long sum = 0;
        foreach (var history in histories)
        {
            if (history.TryGetValue(combination, out var result)) ;
            {
                sum += result;
            }
        }
        return sum;
    }

    private static List<(int Cost, int Change)> IterateSecret(int start)
    {
        long secret = start;
        List<(int Cost, int Change)> history = [((int)secret % 10, int.MaxValue)];
        for (int i = 0; i < iterations; i++)
        {
            secret = CalculateNext(secret);
            var value = (int)secret % 10;
            history.Add((value, value - history[i].Cost));
        }
        return history;
    }

    private static Dictionary<string, int> DictifyHistory(List<(int Cost, int Change)> history)
    {
        Dictionary<string, int> dict = [];

        for (int i = 4; i <= iterations; i++)
        {
            string historySequence = $"{history[i - 3].Change},{history[i - 2].Change},{history[i - 1].Change},{history[i].Change}";
            dict.TryAdd(historySequence, history[i].Cost);
        }
        return dict;
    }

    private static long CalculateNext(long secret)
    {
        var newSecret = secret;

        long times64 = newSecret * 64;
        newSecret ^= times64;
        newSecret %= modulo;

        long by32 = newSecret / 32;
        newSecret ^= by32;
        newSecret %= modulo;

        long times2024 = newSecret * 2048;
        newSecret ^= times2024;
        return newSecret % modulo;
    }
}
