namespace Qwirkle.UltraBoardGames.Player;

public static class ExtensionMethods
{
    public static UltraBoardGamesTileImageCode ToCode(this Tile tile)
    {
        var (tileColor, tileShape) = tile;
        var colorCode = UltraBoardGamesTileImageCode.Colors.First(c => c.Value == tileColor).Key;
        var shapeCode = UltraBoardGamesTileImageCode.Shapes.First(c => c.Value == tileShape).Key;
        return new UltraBoardGamesTileImageCode($"{colorCode}{shapeCode}");
    }
}
