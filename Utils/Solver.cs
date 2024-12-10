using System.Diagnostics;
using System.Reflection;

namespace Utils;

public static class Solver
{
    private const string InputFileName = "input.txt";
    private const string SampleFileName = "sample.txt";
    private const string SampleResultFileName = "sampleResult.txt";

    public static void Solve<T>(Func<T, long> solveFunc, bool skipSample = false)
    {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        string inputPath = Path.Combine(basePath, InputFileName);
        string samplePath = Path.Combine(basePath, SampleFileName);
        string sampleResultPath = Path.Combine(basePath, SampleResultFileName);

        T input = ParseInput<T>(inputPath);
        T sample = ParseInput<T>(samplePath);
        bool parsedResult = long.TryParse(File.ReadAllText(sampleResultPath), out long sampleResult);

        if (input == null || sample == null || !parsedResult)
        {
            Console.WriteLine("Invalid inputs.");
            return;
        }

        ProcessSolve(solveFunc, sample, sampleResult, input, skipSample);
    }

    private static T ParseInput<T>(string path)
    {
        if (typeof(T) == typeof(string[]))
        {
            return (T)(object)File.ReadAllLines(path);
        }
        if (typeof(T) == typeof(string))
        {
            return (T)(object)File.ReadAllText(path);
        }
        throw new InvalidOperationException($"Unsupported input type: {typeof(T)}");
    }

    private static void ProcessSolve<T>(Func<T, long> solveFunc, T sample, long sampleResult, T input, bool skipSample)
    {
        var sw = new Stopwatch();

        if (!skipSample)
        {
            sw.Start();
            var actualSampleResult = solveFunc(sample);
            if (actualSampleResult != sampleResult)
            {
                Console.WriteLine($"Provided result: {sampleResult}, computed result: {actualSampleResult}.");
                Console.WriteLine("Sample result does not match, terminating.");
                return;
            }
            sw.Stop();
            Console.WriteLine($"Sample result confirmed in {sw.ElapsedMilliseconds} ms.");
        }

        sw.Restart();
        var mainResult = solveFunc(input);
        sw.Stop();

        Console.WriteLine($"Main run finished in: {sw.ElapsedMilliseconds} ms.");
        Console.WriteLine("Result:");
        Console.WriteLine(mainResult);
        Clipboard.SetText(mainResult.ToString());
        Console.WriteLine("Result copied to clipboard.");
    }
}
