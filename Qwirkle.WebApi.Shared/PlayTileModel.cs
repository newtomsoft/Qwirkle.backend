namespace Qwirkle.WebApi.Shared;

[Serializable]
public class PlayTileModel
{
    public int GameId { get; init; }
    public Tile Tile { get; init; }
    public Coordinate Coordinate { get; init; }

    public PlayTileModel() { }

    public PlayTileModel(int gameId, Tile tile, Coordinate coordinate)
    {
        GameId = gameId;
        Tile = tile;
        Coordinate = coordinate;
    }

    public TileOnBoard ToTileOnBoard() => new(Tile, Coordinate);
}