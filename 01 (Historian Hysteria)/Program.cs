using System.Diagnostics;
namespace One;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var inputLines = File.ReadAllLines("input.txt");
        var sampleLines = File.ReadAllLines("sample.txt");
        var parsedResult = long.TryParse(File.ReadAllLines("sampleResult.txt").First(), out long sampleResult);

        if (!parsedResult)
        {
            Console.WriteLine("Could not read sample result.");
            return;
        }

        var sw = new Stopwatch();
        sw.Start();

        var actualSampleResult = Run(sampleLines);
        if (actualSampleResult != sampleResult)
        {
            Console.WriteLine("Sample result does not match, terminating.");
            return;
        }

        sw.Stop();
        Console.WriteLine($"Sample result confirmed in {sw.ElapsedMilliseconds} ms.");

        sw.Restart();
        var mainResult = Run(inputLines);
        sw.Stop();
        Console.WriteLine($"Main run finished in: {sw.ElapsedMilliseconds} ms.");
        Console.WriteLine("Result:");
        Console.WriteLine(mainResult);
        Clipboard.SetText(mainResult.ToString());
        Console.WriteLine("Copied to clipboard.");
    }

    private static long Run(string[] lines)
    {
        List<long> left = [];
        List<long> right = [];

        foreach (string line in lines)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            left.Add(int.Parse(parts[0]));
            right.Add(int.Parse(parts[1]));
        }

        left = [.. left.Order()];
        right = [.. right.Order()];

        var combined = left.Zip(right);
        var result = combined.Aggregate(0, (long total, (long First, long Second) next) => total + Math.Abs(next.First - next.Second));
        return result;
    }
}