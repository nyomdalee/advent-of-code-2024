﻿namespace Utils.Grid;

/// <summary>
/// Grid utility with a 2D array <see cref="Values"/>. [0,0] is bottom left of line input.
/// </summary>
public class Grid()
{
    private Point? lowerBound;
    private Point? upperBound;
    private static IReadOnlyCollection<Direction>? allDirections;

    public char[,] Values { get; set; } = new char[0, 0];

    public Point LowerBound => lowerBound ??= new Point(Values.GetLowerBound(1), Values.GetLowerBound(0));

    public Point UpperBound => upperBound ??= new Point(Values.GetUpperBound(1), Values.GetUpperBound(0));

    public static IReadOnlyCollection<Direction> CardinalDirections { get; } =
    [
        new Direction(0, 1, DirectionName.North),
        new Direction(1, 0, DirectionName.East),
        new Direction(0, -1, DirectionName.South),
        new Direction(-1, 0, DirectionName.West)
    ];

    public static IReadOnlyCollection<Direction> DiagonalDirections { get; } =
    [
        new Direction(1, 1, DirectionName.NorthEast),
        new Direction(1, -1, DirectionName.SouthEast),
        new Direction(-1, -1, DirectionName.SouthWest),
        new Direction(-1, 1, DirectionName.NorthWest)
    ];

    public static IReadOnlyCollection<Direction> AllDirections { get; } = allDirections ??= [.. CardinalDirections, .. DiagonalDirections];

    public static readonly Dictionary<DirectionName, Direction> TurnRight = new()
    {
        { DirectionName.North, GetDirectionByName(DirectionName.East) },
        { DirectionName.East, GetDirectionByName(DirectionName.South) },
        { DirectionName.South, GetDirectionByName(DirectionName.West) },
        { DirectionName.West, GetDirectionByName(DirectionName.North) },
    };

    public static readonly Dictionary<DirectionName, Direction> TurnLeft = new()
    {
        { DirectionName.North, GetDirectionByName(DirectionName.West) },
        { DirectionName.East, GetDirectionByName(DirectionName.North) },
        { DirectionName.South, GetDirectionByName(DirectionName.East) },
        { DirectionName.West, GetDirectionByName(DirectionName.South) },
    };

    public static Grid FromLines(string[]? lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        int rows = lines.Length;
        int cols = lines[0].Length;

        var grid = new Grid { Values = new char[rows, cols] };
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                grid.Values[rows - 1 - i, j] = lines[i][j];
            }
        }
        return grid;
    }

    public static Grid UniformOfSize(int x, int y, char value)
    {
        var grid = new Grid { Values = new char[x, y] };

        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                grid.Values[i, j] = value;
            }
        }
        return grid;
    }

    // TODO: this is dumb but necessary
    public char GetValue(int X, int Y) => Values[Y, X];
    public char GetValue(Point point) => Values[point.Y, point.X];
    public char GetValueAfterMove(Point point, Direction direction, int steps = 1) => Values[point.Y + (direction.Y * steps), point.X + (direction.X * steps)];
    public static Point Step(Point point, Direction direction, int steps = 1) => new(point.X + (direction.X * steps), point.Y + (direction.Y * steps));
    public void SetValue(Point point, char value) => Values[point.Y, point.X] = value;
    public void SetValueAfterMove(Point point, Direction direction, char value, int steps = 1) => Values[point.Y + (direction.Y * steps), point.X + (direction.X * steps)] = value;

    public bool IsOutOfBounds(Point point, Direction direction, int steps = 1)
    {
        return (point.X + (direction.X * steps)) < LowerBound.X || (point.X + (direction.X * steps)) > UpperBound.X ||
               (point.Y + (direction.Y * steps)) < LowerBound.Y || (point.Y + (direction.Y * steps)) > UpperBound.Y;
    }

    public bool IsOutOfBounds(Point point)
    {
        return point.X < LowerBound.X || point.X > UpperBound.X ||
               point.Y < LowerBound.Y || point.Y > UpperBound.Y;
    }

    public bool IsOutOfBounds(int x, int y)
    {
        return x < LowerBound.X || x > UpperBound.X ||
               y < LowerBound.Y || y > UpperBound.Y;
    }

    public IEnumerable<Point> Expand(Point point, IEnumerable<Direction> directions)
    {
        foreach (var direction in directions)
        {
            var newPoint = new Point(point.X + direction.X, point.Y + direction.Y);
            if (!IsOutOfBounds(newPoint))
            {
                yield return newPoint;
            }
        }
    }

    public static Direction GetDirectionByName(DirectionName directionName)
        => AllDirections.First(x => x.Name == directionName);

    public Grid DeepCleanCopy()
    {
        int rows = UpperBound.Y;
        int columns = UpperBound.X;

        char[,] copy = new char[rows + 1, columns + 1];
        for (int i = 0; i <= rows; i++)
        {
            for (int j = 0; j <= columns; j++)
            {
                copy[i, j] = Values[i, j];
            }
        }

        return new Grid { Values = copy };
    }

    public IEnumerable<Point> GetPointsByPredicate(Func<char, bool> func)
    {
        int rows = UpperBound.Y;
        int columns = UpperBound.X;

        for (int x = 0; x <= columns; x++)
        {
            for (int y = 0; y <= rows; y++)
            {
                var element = Values[y, x];
                if (func(element))
                {
                    yield return new Point(x, y);
                }
            }
        }
    }

    public IEnumerable<TResult> LoopWithIndices<TResult>(Func<int, int, char, TResult> func)
    {
        int rows = UpperBound.Y;
        int columns = UpperBound.X;

        for (int x = 0; x <= columns; x++)
        {
            for (int y = 0; y <= rows; y++)
            {
                var element = Values[y, x];
                yield return func(x, y, element);
            }
        }
    }

    public void DebugPrint()
    {
        for (int i = Values.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < Values.GetLength(1); j++)
            {
                Console.Write(Values[i, j]);
            }
            Console.WriteLine();
        }
    }

    public void DebugPrintInverse()
    {
        for (int i = 0; i < Values.GetLength(0); i++)
        {
            for (int j = 0; j < Values.GetLength(1); j++)
            {
                Console.Write(Values[i, j]);
            }
            Console.WriteLine();
        }
    }
}
