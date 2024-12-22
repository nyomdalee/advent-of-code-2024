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


        return secrets.Sum(IterateSecret);
    }

    private static long IterateSecret(int start)
    {
        long secret = start;

        for (int i = 0; i < iterations; i++)
        {
            secret = CalculateNext(secret);
        }
        return secret;
    }

    private static long CalculateNext(long secret)
    {
        var yep = secret;

        // 1
        long times64 = yep * 64;
        yep = yep ^ times64;
        yep = yep % modulo;

        // 2
        long by32 = yep / 32;
        yep = yep ^ by32;
        yep = yep % modulo;

        // 3
        long times2024 = yep * 2048;
        yep = yep ^ times2024;
        yep = yep % modulo;

        return yep;
    }
}
