namespace Qwirkle.Domain.ValueObjects;

public record Rack(List<TileOnRack> Tiles)
{
    public static Rack From(IEnumerable<TileOnRack> tiles) => new(tiles.ToList());
    public static Rack Empty => From(new List<TileOnRack>());
    public int TilesNumber => Tiles?.Count ?? 0;

    public Rack WithoutDuplicatesTiles()
    {
        var tiles = Tiles.Select(t => t.ToTile()).Distinct();
        return new(tiles.Select((t, index) => t.ToTileOnRack((RackPosition)index)).ToList());
    }
    
    public Rack ToHiddenRack() => From(Tiles.Select(t => t.ToHiddenTile()));
}