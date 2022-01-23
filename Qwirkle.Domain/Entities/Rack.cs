namespace Qwirkle.Domain.Entities;

public record Rack(List<TileOnPlayer> Tiles)
{
    public static Rack From(List<TileOnPlayer> tiles) => new(tiles);
    public static Rack Empty => From(new List<TileOnPlayer>());
    
    public Rack WithoutDuplicatesTiles()
    {
        var tiles = Tiles.Select(t => t.ToTile()).Distinct().ToList();
        return new(tiles.Select((t, index) => t.ToTileOnPlayer((RackPosition)index)).ToList());
    }
}