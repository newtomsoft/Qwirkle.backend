namespace Qwirkle.Domain.ValueObjects;

public record SwapTilesReturn(int GameId, ReturnCode Code, Rack NewRack);