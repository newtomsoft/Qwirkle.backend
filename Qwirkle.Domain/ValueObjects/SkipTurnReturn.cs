namespace Qwirkle.Domain.ValueObjects;

public struct SwapTilesReturn
{
    public int GameId { get; set; }
    public ReturnCode Code { get; set; }
    public Rack NewRack { get; set; }
}