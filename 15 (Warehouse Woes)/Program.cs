using Utils;
using Utils.Grid;

namespace Fifteen;

public static class Program
{
    private const char Robot = '@';
    private const char Box = 'O';
    private const char BoxLeft = '[';
    private const char BoxRight = ']';
    private const char Wall = '#';
    private const char Empty = '.';

    private static readonly Dictionary<char, Direction> dirTranslate = new()
    {
        { 'v', Grid.GetDirectionByName(DirectionName.North) },
        { '>', Grid.GetDirectionByName(DirectionName.East) },
        { '^', Grid.GetDirectionByName(DirectionName.South) },
        { '<', Grid.GetDirectionByName(DirectionName.West) },
    };

    [STAThread]
    public static void Main()
    {
        Solver.Solve<string, long>(Run);
    }

    private static long Run(string lines)
    {
        var yek = lines.Split("\r\n\r\n");

        var gridLines = yek[0].Split("\r\n").ToList();
        List<string> expandedLines = [];
        foreach (var gridLine in gridLines)
        {
            var e = gridLine;
            e = e.Replace("#", "##");
            e = e.Replace("O", "[]");
            e = e.Replace(".", "..");
            e = e.Replace("@", "@.");
            expandedLines.Add(e);
        }
        expandedLines.Reverse();
        var grid = Grid.FromLines(expandedLines.ToArray());

        var distr = yek[1];
        distr = distr.Replace(Environment.NewLine, "");
        List<Direction> moves = [];
        foreach (var chara in distr)
        {
            moves.Add(dirTranslate[chara]);
        }

        var robitPosition = grid.GetPointsByPredicate(x => Equals(x, Robot)).First();
        PlaySokoban(grid, robitPosition, moves);

        var boxes = grid.GetPointsByPredicate(x => Equals(x, BoxLeft)).ToList();
        return boxes.Sum(x => (x.Y * 100) + x.X);
    }

    private static void PlaySokoban(Grid grid, Point robit, List<Direction> moves)
    {
        foreach (var move in moves)
        {
            var valueAfter = grid.GetValueAfterMove(robit, move);

            if (Equals(valueAfter, Empty))
            {
                robit = MoveRobit(grid, robit, move);
            }

            if (Equals(valueAfter, BoxLeft) || Equals(valueAfter, BoxRight))
            {
                if (move.Name is DirectionName.East or DirectionName.West)
                {
                    if (TryPushHorizontal(grid, robit, move))
                    {
                        robit = MoveRobit(grid, robit, move);
                    }
                }
                else
                {
                    if (TryPushVertical(grid, robit, move))
                    {
                        robit = MoveRobit(grid, robit, move);
                    }
                }
            }
        }
    }

    private static bool TryPushVertical(Grid grid, Point robit, Direction move)
    {
        var currentLayer = ExpandBox(grid, Grid.Step(robit, move));
        List<Point> allToMove = [];

        while (currentLayer.Count > 0)
        {
            allToMove.AddRange(currentLayer);
            List<Point> nextLayer = [];

            foreach (var piece in currentLayer)
            {
                var value = grid.GetValueAfterMove(piece, move);

                if (Equals(value, Wall))
                {
                    return false;
                }

                if (Equals(value, BoxLeft) || Equals(value, BoxRight))
                {
                    nextLayer.AddRange(ExpandBox(grid, Grid.Step(piece, move)));
                }
            }

            currentLayer = nextLayer;
        }
        PushBoxesVertical(grid, allToMove, move);
        return true;
    }

    private static void PushBoxesVertical(Grid grid, List<Point> boxes, Direction move)
    {
        var toMove = boxes.ConvertAll(x => (Point: x, Value: grid.GetValue(x)));
        List<Point> newPositions = [];
        foreach (var box in toMove)
        {
            var nP = Grid.Step(box.Point, move);
            newPositions.Add(nP);
            grid.SetValue(nP, box.Value);
        }

        var newEmpty = boxes.Except(newPositions).ToList();
        foreach (var empty in newEmpty)
        {
            grid.SetValue(empty, Empty);
        }
    }

    private static bool TryPushHorizontal(Grid grid, Point robit, Direction move)
    {
        var boxes = 1;
        while (true)
        {
            boxes += 2;
            var value = grid.GetValueAfterMove(robit, move, boxes);

            if (Equals(value, Wall))
            {
                return false;
            }

            if (Equals(value, Empty))
            {
                PushBoxesHorizontal(grid, robit, move, boxes);
                return true;
            }
        }
    }

    private static void PushBoxesHorizontal(Grid grid, Point robit, Direction move, int boxes)
    {
        var first = move.X == -1 ? BoxLeft : BoxRight;
        var second = move.X == -1 ? BoxRight : BoxLeft;

        for (var i = 0; i <= boxes; i++)
        {
            grid.SetValueAfterMove(robit, move, i % 2 == 1 ? first : second, i);
        }
    }

    private static Point MoveRobit(Grid grid, Point robit, Direction move)
    {
        grid.SetValue(robit, Empty);
        robit = Grid.Step(robit, move);
        grid.SetValue(robit, Robot);
        return robit;
    }

    private static List<Point> ExpandBox(Grid grid, Point boxPart) =>
        [boxPart, new(boxPart.X + (Equals(grid.GetValue(boxPart), BoxLeft) ? 1 : -1), boxPart.Y)];
}
