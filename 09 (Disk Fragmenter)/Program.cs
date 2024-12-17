using Utils;

namespace Nine;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run);
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

        bool[] alreadyTouched = new bool[expanded.Where(x => x is not null).Max(x => x.Value + 1)];

        for (int i = expanded.Length - 1; i >= 0; i--)
        {
            var nextId = expanded[i];
            if (nextId is not null)
            {
                {
                    if (alreadyTouched[(int)nextId])
                    {
                        continue;
                    }
                }

                var blockLength = GetBlockLength(expanded, i, (int)nextId);

                var nextSlot = FindSlotOfLength(expanded, blockLength, i);
                alreadyTouched[(int)nextId] = true;

                if (nextSlot == -1)
                {
                    continue;
                }

                MoveBlock(expanded, i, blockLength, nextSlot, (int)nextId);

            }
        }

        return expanded
            .Select((value, index) => new { value, index })
            .Where(x => x.value is not null)
            .Aggregate(0L, (current, next) => current + (long)(next.value * next.index));
    }
    private static void MoveBlock(int?[] expanded, int current, int length, int slotStart, int value)
    {
        for (int i = 0; i < length; i++)
        {
            expanded[slotStart + i] = value;
        }

        for (int i = 0; i < length; i++)
        {
            expanded[current - i] = null;
        }
    }

    private static int FindSlotOfLength(int?[] expanded, int length, int max)
    {
        for (int i = 0; i < expanded.Length; i++)
        {
            if (i >= max)
            {
                break;
            }

            if (expanded[i] == null && CanFitBlock(expanded, i, length))
            {
                return i;
            }
        }
        return -1;
    }

    private static bool CanFitBlock(int?[] expanded, int current, int length)
    {
        for (int i = current; i < current + length; i++)
        {
            if (expanded[i] is not null)
            {
                return false;
            }
        }
        return true;
    }

    private static int GetBlockLength(int?[] expanded, int i, int nextId)
    {
        int length = 0;
        while (i >= 0 && expanded[i] == nextId)
        {
            length++;
            i--;
        }
        return length;
    }
}
