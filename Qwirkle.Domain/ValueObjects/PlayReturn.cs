namespace Qwirkle.Domain.ValueObjects;

public record PlayReturn(int GameId, ReturnCode Code, Move Move, Rack NewRack);
