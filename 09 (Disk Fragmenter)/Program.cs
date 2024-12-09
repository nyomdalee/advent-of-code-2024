using Utils;

namespace Nine;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string>(Run);
    }

    private static long Run(string? line)
    {
        var numbers = line.Select(x => int.Parse(x.ToString())).ToArray();

        var arrLength = numbers.Sum(x => x);
        int?[] expanded = new int?[arrLength];

        int oogaIndex = 0;
        int numberId = 0;

        for (int i = 0; i < numbers.Length; i++)
        {
            for (int j = 0; j < numbers[i]; j++)
            {
                if (i % 2 == 0)
                {
                    expanded[oogaIndex] = numberId;
                }
                oogaIndex++;
            }
            if (i % 2 == 0)
            {
                numberId++;
            }
        }

        for (int i = expanded.Length - 1; i >= 0; i--)
        {
            //DebugPrint(expanded);

            if (expanded[i] is not null)
            {
                var nextSlot = FindSlot(expanded, i);

                if (nextSlot == -1)
                {
                    break;
                }

                expanded[nextSlot] = expanded[i];
                expanded[i] = null;
            }
        }


        var total = expanded
            .Select((value, index) => new { value, index })
            .Where(x => x.value is not null)
            .Aggregate(0L, (current, next) => current + (long)(next.value * next.index));

        return total;
    }

    private static int FindSlot(int?[] expanded, int max)
    {
        for (int i = 0; i < expanded.Length; i++)
        {
            if (i >= max)
            {
                break;
            }

            if (expanded[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    private static void DebugPrint(int?[] fuck)
    {
        string result = string.Join(" ", fuck.Select(n => n.HasValue ? n.ToString() : "."));
        Console.WriteLine(result);
    }
}
