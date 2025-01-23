namespace GameLogic.Round;

public struct Coordinate(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}