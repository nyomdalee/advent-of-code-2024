using Utils;

namespace One;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[]>(Run);
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

        var result = left.Sum(x => x * right.Count(y => y == x));
        return result;
    }
}