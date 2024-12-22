using Utils;

namespace TwentyOne;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var f = new RobotSolver();
        Solver.Solve<string[], long>(f.Run);
    }
}