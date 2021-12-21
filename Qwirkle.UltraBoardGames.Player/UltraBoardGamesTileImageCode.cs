namespace Qwirkle.UltraBoardGames.Player;

public record UltraBoardGamesTileImageCode
{
    private readonly string _value;

    public UltraBoardGamesTileImageCode(string value)
    {
        if (value.Length != 2) throw new ArgumentException("value length must be 2");
        _value = value;
    }

    public Tile ToTile() => new(Colors[_value[0]], Shapes[_value[1]]);

    public static readonly Dictionary<char, TileColor> Colors = new()
    {
        { 'b', TileColor.Blue },
        { 'g', TileColor.Green },
        { 'o', TileColor.Orange },
        { 'p', TileColor.Purple },
        { 'r', TileColor.Red },
        { 'y', TileColor.Yellow }
    };

    public static readonly Dictionary<char, TileShape> Shapes = new()
    {
        { '1', TileShape.FourPointStar },
        { '2', TileShape.Clover },
        { '3', TileShape.EightPointStar },
        { '4', TileShape.Diamond },
        { '5', TileShape.Square },
        { '6', TileShape.Circle }
    };
}
