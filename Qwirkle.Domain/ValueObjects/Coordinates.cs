namespace Qwirkle.Domain.ValueObjects;

public record Coordinates(Abscissa X, Ordinate Y) : IComparable
{
    public static Coordinates From(int x, int y) => new((sbyte)x, (sbyte)y);
    public Coordinates Right() => new((Abscissa)(X + 1), Y);
    public Coordinates Left() => new((Abscissa)(X - 1), Y);
    public Coordinates Top() => new(X, (Ordinate)(Y + 1));
    public Coordinates Bottom() => new(X, (Ordinate)(Y - 1));
    public int CompareTo(object obj) => obj is not Coordinates(var x, var y) ? 1 : X != x ? X.CompareTo(x) : Y.CompareTo(y);

    public override string ToString() => $"({X},{Y})";
}