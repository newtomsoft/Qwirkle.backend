namespace Qwirkle.WebApi.Shared;

[Serializable]
public class SwapTileModel
{
    public int GameId { get; init; }
    public Tile Tile { get; init; }
    public RackPosition RackPosition { get; init; }

    public SwapTileModel() { }

    public SwapTileModel(int gameId, Tile tile, RackPosition rackPosition)
    {
        GameId = gameId;
        Tile = tile;
        RackPosition = rackPosition;
    }

    public TileOnRack ToTileOnRack() => new(RackPosition, Tile);
}