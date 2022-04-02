namespace Qwirkle.Domain.ValueObjects;

public record Coordinate(Abscissa X, Ordinate Y) : IComparable
{
    public static Coordinate From(int x, int y) => new((sbyte)x, (sbyte)y);
    public Coordinate Right() => new((Abscissa)(X + 1), Y);
    public Coordinate Left() => new((Abscissa)(X - 1), Y);
    public Coordinate Top() => new(X, (Ordinate)(Y + 1));
    public Coordinate Bottom() => new(X, (Ordinate)(Y - 1));
    public int CompareTo(object obj) => obj is not Coordinate(var x, var y) ? 1 : X != x ? X.CompareTo(x) : Y.CompareTo(y);

    public override string ToString() => $"({X},{Y})";
}