namespace Qwirkle.Domain.ValueObjects;

public record Coordinates(Abscissa X, Ordinate Y)
{
    public static Coordinates From(int x, int y) => new((sbyte)x, (sbyte)y);
    public Coordinates Right() => new((Abscissa)(X + 1), Y);
    public Coordinates Left() => new((Abscissa)(X - 1), Y);
    public Coordinates Top() => new(X, (Ordinate)(Y + 1));
    public Coordinates Bottom() => new(X, (Ordinate)(Y - 1));
}