namespace Qwirkle.Core.ValueObjects;

public struct PlayReturn
{
    public int GameId { get; set; }
    public PlayReturnCode Code { get; set; }
    public List<TileOnBoard> TilesPlayed { get; set; }
    public Rack NewRack { get; set; }
    public int Points { get; set; }
}
