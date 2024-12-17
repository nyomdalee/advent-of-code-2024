using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Utils;

public static class Solver
{
    private const string InputFileName = "input.txt";
    private const string SampleFileName = "sample.txt";
    private const string SampleResultFileName = "sampleResult.txt";

    public static void Solve<TInput, TOutput>(Func<TInput, TOutput> solveFunc, bool skipSample = false)
    {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        string inputPath = Path.Combine(basePath, InputFileName);
        string samplePath = Path.Combine(basePath, SampleFileName);
        string sampleResultPath = Path.Combine(basePath, SampleResultFileName);

        TInput input = ParseInput<TInput>(inputPath);
        TInput sample = ParseInput<TInput>(samplePath);
        string sampleResultContent = File.ReadAllText(sampleResultPath).Trim();
        TOutput sampleResult;
        try
        {
            sampleResult = ParseOutput<TOutput>(sampleResultContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to parse sample result: {ex.Message}");
            return;
        }

        if (input == null || sample == null)
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

    private static TOutput ParseOutput<TOutput>(string value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(TOutput))
            ?? throw new InvalidOperationException($"No type converter available for type {typeof(TOutput)}.");

        if (!converter.IsValid(value))
        {
            throw new InvalidCastException($"The value '{value}' is not valid for type {typeof(TOutput)}.");
        }

        var result = converter.ConvertFromString(value);
        return result is null
            ? throw new InvalidCastException($"Failed to convert '{value}' to type {typeof(TOutput)}.")
            : (TOutput)result;
    }

    private static void ProcessSolve<TInput, TOutput>(Func<TInput, TOutput> solveFunc, TInput sample, TOutput sampleResult, TInput input, bool skipSample)
    {
        var sw = new Stopwatch();

        if (!skipSample)
        {
            sw.Start();
            var actualSampleResult = solveFunc(sample);
            if (!Equals(actualSampleResult, sampleResult))
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
