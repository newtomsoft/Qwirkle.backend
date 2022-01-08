namespace Qwirkle.Domain.ValueObjects;

public struct SwapTilesReturn
{
    public int GameId { get; set; }
    public PlayReturnCode Code { get; set; }
    public Rack NewRack { get; set; }
}