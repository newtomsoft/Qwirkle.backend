using Qwirkle.Domain.Enums;
using Qwirkle.Domain.ValueObjects;

namespace Qwirkle.UltraBoardGames.Player;

public static class ExtensionsMethods
{
    public static Tile ToTile(this string imageCode)
    {
        Dictionary<char, TileColor> colors = new()
        {
            { 'b', TileColor.Blue },
            { 'g', TileColor.Green },
            { 'o', TileColor.Orange },
            { 'p', TileColor.Purple },
            { 'r', TileColor.Red },
            { 'y', TileColor.Yellow }
        };
        Dictionary<char, TileShape> shapes = new()
        {
            { '1', TileShape.FourPointStar },
            { '2', TileShape.Clover },
            { '3', TileShape.EightPointStar },
            { '4', TileShape.Diamond },
            { '5', TileShape.Square },
            { '6', TileShape.Circle }
        };
        return new Tile(colors[imageCode[0]], shapes[imageCode[1]]);   
    }
}
