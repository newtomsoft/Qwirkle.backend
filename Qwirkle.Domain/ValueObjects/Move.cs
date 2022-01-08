namespace Qwirkle.Domain.ValueObjects;

public record Move(IEnumerable<TileOnBoard> Tiles, int Points);