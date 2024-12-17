using Utils;

namespace Eleven;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run, true);
    }

    private static long Run(string? line)
    {
        List<long> rocks = line.Split(' ').ToList().ConvertAll(long.Parse);
        Dictionary<long, long> rockDict = rocks.GroupBy(x => x).ToDictionary(x => x.Key, x => (long)x.Count());

        for (long i = 0; i < 75; i++)
        {
            Dictionary<long, long> newRocks = [];
            foreach (var rock in rockDict)
            {
                HandleRock(rock, newRocks);
            }
            rockDict = newRocks;
        }
        return rockDict.Sum(x => x.Value);
    }

    private static void HandleRock(KeyValuePair<long, long> rock, Dictionary<long, long> newRocks)
    {
        void AddOrUpdate(long key, long value)
        {
            if (newRocks.ContainsKey(key))
            {
                newRocks[key] += value;
            }
            else
            {
                newRocks[key] = value;
            }
        }

        if (rock.Key == 0)
        {
            AddOrUpdate(1, rock.Value);
        }
        else if ((rock.Key.ToString().Length % 2) == 0)
        {
            int half = rock.Key.ToString().Length / 2;

            AddOrUpdate(long.Parse(rock.Key.ToString()[..half]), rock.Value);
            AddOrUpdate(long.Parse(rock.Key.ToString()[half..]), rock.Value);
        }
        else
        {
            AddOrUpdate(rock.Key * 2024, rock.Value);
        }
    }
}