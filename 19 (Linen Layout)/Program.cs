using Utils;

namespace Nineteen;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Solver.Solve<string[], long>(Run, true);
    }

    private static long Run(string[] lines)
    {
        string[] towels = lines[0].Split(", ");
        string[] requests = lines[2..^0];

        return requests.Count(x => TryFill(x, towels));
    }
    private static bool TryFill(string request, string[] towels)
    {
        List<string> current = [string.Empty];

        while (current.Count > 0)
        {
            List<string> next = [];
            foreach (var str in current)
            {
                foreach (var towel in towels)
                {
                    string tryString = str + towel;
                    if (tryString.Length > request.Length)
                    {
                        continue;
                    }

                    var sub = request[..(tryString.Length)];

                    if (Equals(tryString, sub))
                    {
                        if (tryString.Length == request.Length)
                        {
                            return true;
                        }
                        else
                        {
                            next.Add(tryString);
                        }
                    }

                }
            }

            current = next.Distinct().ToList();
        }

        return false;
    }
}
