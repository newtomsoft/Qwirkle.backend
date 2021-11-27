namespace Qwirkle.Core.ValueObjects;

public record Move(IEnumerable<TileOnBoard> Tiles, int Points);
