using Utils;
using Utils.Grid;

namespace TwentyFive;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run);
    }

    public static long Run(string text)
    {
        var yek = text.Split("\r\n\r\n").Select(x => x.Split("\r\n"));
        List<Grid> grids = yek.Select(x => Grid.FromLines(x)).ToList();

        var keys = grids.Where(x => Equals(x.GetValue(0, 0), '#')).ToList();
        var locks = grids.Except(keys).ToList();

        var keyHeights = GetHeights(keys);
        var lockHeights = GetHeights(locks);

        return TryLocks(keyHeights, lockHeights);
    }

    private static long TryLocks(List<List<int>> keys, List<List<int>> lcoks)
    {
        long total = 0;
        foreach (var keyHeights in keys)
        {
            foreach (var lockHeights in lcoks)
            {
                if (TrySingle(keyHeights, lockHeights))
                {
                    total++;
                }
            }
        }
        return total;
    }

    private static bool TrySingle(List<int> keyHeights, List<int> lockHeights)
    {
        for (var i = 0; i < lockHeights.Count; i++)
        {
            if (lockHeights[i] + keyHeights[i] > 5)
            {
                return false;
            }
        }
        return true;
    }

    private static List<List<int>> GetHeights(List<Grid> set)
    {
        List<List<int>> setHeights = [];
        foreach (var item in set)
        {
            List<int> itemHeights = [];
            for (var x = 0; x <= item.UpperBound.X; x++)
            {
                int columnCount = 0;
                for (var y = 0; y <= item.UpperBound.Y; y++)
                {
                    if (Equals(item.GetValue(x, y), '#'))
                    {
                        columnCount++;
                    }
                }
                itemHeights.Add(columnCount - 1);
            }
            setHeights.Add(itemHeights);
        }
        return setHeights;
    }
}