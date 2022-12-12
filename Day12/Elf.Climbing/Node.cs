namespace Elf.Climbing;

public record struct Node(Point Location, char Height, List<Point> Connections, Point Parent);

public readonly record struct Point(int X, int Y);