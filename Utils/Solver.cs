using System.Diagnostics;
using System.Reflection;

namespace Utils;

public static class Solver
{
    public static void Solve(
        Func<string[], long> solveFunc,
        string inputFileName = "input.txt",
        string sampleFileName = "sample.txt",
        string sampleResultFileName = "sampleResult.txt")
    {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        string inputPath = Path.Combine(basePath, inputFileName);
        string samplePath = Path.Combine(basePath, sampleFileName);
        string sampleResultPath = Path.Combine(basePath, sampleResultFileName);

        var inputLines = File.ReadAllLines(inputPath);
        var sampleLines = File.ReadAllLines(samplePath);
        var parsedResult = long.TryParse(File.ReadAllText(sampleResultPath), out long sampleResult);

        if (inputLines.Length == 0 || sampleLines.Length == 0 || !parsedResult)
        {
            Console.WriteLine("Invalid inputs.");
            return;
        }

        var sw = new Stopwatch();

        sw.Start();
        var actualSampleResult = solveFunc(sampleLines);
        if (actualSampleResult != sampleResult)
        {
            Console.WriteLine($"Provided result: {sampleResult}, computed result: {actualSampleResult}.");
            Console.WriteLine("Sample result does not match, terminating.");
            return;
        }
        sw.Stop();
        Console.WriteLine($"Sample result confirmed in {sw.ElapsedMilliseconds} ms.");

        sw.Restart();
        var mainResult = solveFunc(inputLines);
        sw.Stop();

        Console.WriteLine($"Main run finished in: {sw.ElapsedMilliseconds} ms.");
        Console.WriteLine("Result:");
        Console.WriteLine(mainResult);
        Clipboard.SetText(mainResult.ToString());
        Console.WriteLine("Result copied to clipboard.");
    }

    public static void Solve(
        Func<string?, long> solveFunc,
        string inputFileName = "input.txt",
        string sampleFileName = "sample.txt",
        string sampleResultFileName = "sampleResult.txt")
    {
        string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;
        string inputPath = Path.Combine(basePath, inputFileName);
        string samplePath = Path.Combine(basePath, sampleFileName);
        string sampleResultPath = Path.Combine(basePath, sampleResultFileName);

        var inputLines = File.ReadAllText(inputPath);
        var sampleLines = File.ReadAllText(samplePath);
        var parsedResult = long.TryParse(File.ReadAllText(sampleResultPath), out long sampleResult);

        if (inputLines.Length == 0 || sampleLines.Length == 0 || !parsedResult)
        {
            Console.WriteLine("Invalid inputs.");
            return;
        }

        var sw = new Stopwatch();

        sw.Start();
        var actualSampleResult = solveFunc(sampleLines);
        if (actualSampleResult != sampleResult)
        {
            Console.WriteLine($"Provided result: {sampleResult}, computed result: {actualSampleResult}.");
            Console.WriteLine("Sample result does not match, terminating.");
            return;
        }
        sw.Stop();
        Console.WriteLine($"Sample result confirmed in {sw.ElapsedMilliseconds} ms.");

        sw.Restart();
        var mainResult = solveFunc(inputLines);
        sw.Stop();

        Console.WriteLine($"Main run finished in: {sw.ElapsedMilliseconds} ms.");
        Console.WriteLine("Result:");
        Console.WriteLine(mainResult);
        Clipboard.SetText(mainResult.ToString());
        Console.WriteLine("Result copied to clipboard.");
    }
}
