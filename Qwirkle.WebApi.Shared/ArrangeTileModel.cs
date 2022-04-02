namespace Qwirkle.WebApi.Shared;

[Serializable]
public class ArrangeTileModel
{
    public int GameId { get; init; }
    public Tile Tile { get; init; }
    public RackPosition RackPosition { get; init; }


    public ArrangeTileModel() { }

    public ArrangeTileModel(int gameId, Tile tile, RackPosition rackPosition)
    {
        GameId = gameId;
        Tile = tile;
        RackPosition = rackPosition;
    }

    public TileOnRack ToTileOnRack() => new(RackPosition, Tile);
}
