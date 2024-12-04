namespace Utils.Grid;

public class Point(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public Point Move(Direction direction)
    {
        return new(X + direction.X, Y + direction.Y);
    }
}
