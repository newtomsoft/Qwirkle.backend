namespace Qwirkle.Domain.ValueObjects;

public struct SkipTurnReturn
{
    public int GameId { get; set; }
    public ReturnCode Code { get; set; }
}