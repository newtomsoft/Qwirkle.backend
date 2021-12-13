namespace Qwirkle.Domain.Entities;

public record Rack(List<TileOnPlayer> Tiles)
{
    public static Rack From(List<TileOnPlayer> tiles) => new(tiles);
    public Rack WithoutDuplicatesTiles() => new(Tiles.Distinct().ToList());
}