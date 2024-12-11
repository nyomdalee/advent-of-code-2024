using Utils;

namespace Eleven;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string>(Run);
    }

    private static long Run(string? line)
    {
        List<long> rocks = line.Split(' ').ToList().ConvertAll(long.Parse);

        for (long i = 0; i < 25; i++)
        {
            List<long> newRocks = [];
            foreach (var rock in rocks)
            {
                HandleRock(rock, newRocks);
            }
            rocks = newRocks;
        }

        return rocks.Count;
    }

    private static void HandleRock(long rock, List<long> newRocks)
    {
        if (rock == 0)
        {
            newRocks.Add(1);
        }
        else if ((rock.ToString().Length % 2) == 0)
        {
            var rockstring = rock.ToString();
            var half = rockstring.Length / 2;
            newRocks.Add(long.Parse(rockstring[..half]));
            newRocks.Add(long.Parse(rockstring[half..]));
        }
        else
        {
            newRocks.Add(rock * 2024);
        }
    }
}