namespace Qwirkle.UltraBoardGames.Player;

public static class ExtensionsMethods
{
    private static readonly Dictionary<char, TileColor> Colors = new()
    {
        { 'b', TileColor.Blue },
        { 'g', TileColor.Green },
        { 'o', TileColor.Orange },
        { 'p', TileColor.Purple },
        { 'r', TileColor.Red },
        { 'y', TileColor.Yellow }
    };
    private static readonly Dictionary<char, TileShape> Shapes = new()
    {
        { '1', TileShape.FourPointStar },
        { '2', TileShape.Clover },
        { '3', TileShape.EightPointStar },
        { '4', TileShape.Diamond },
        { '5', TileShape.Square },
        { '6', TileShape.Circle }
    };
    public static Tile ToTile(this string imageCode) => new(Colors[imageCode[0]], Shapes[imageCode[1]]);

    public static string ToCode(this Tile tile)
    {
        char colorCode = Colors.First(c => c.Value == tile.Color).Key;
        char shapeCode = Shapes.First(c => c.Value == tile.Shape).Key;
        return $"{colorCode}{shapeCode}";
    }
}
