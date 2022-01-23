namespace Qwirkle.Domain.ValueObjects;

public record Move(List<TileOnBoard> Tiles, int Points)
{
    public static Move Empty => new Move();
    
    private Move() : this(new List<TileOnBoard>(), 0) { }
}