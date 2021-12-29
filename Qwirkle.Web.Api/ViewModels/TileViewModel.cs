namespace Qwirkle.Web.Api.ViewModels;

public class TileViewModel
{
    public int GameId { get; set; }
    public TileColor Color { get; set; }
    public TileShape Shape { get; set; }
    public sbyte X { get; set; }
    public sbyte Y { get; set; }

    public TileOnBoard ToTileOnBoard() => new(Color, Shape, Coordinates.From(X, Y));
    public Tile ToTile() => new(Color, Shape);
}
