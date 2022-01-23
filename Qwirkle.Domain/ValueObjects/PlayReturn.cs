namespace Qwirkle.Domain.ValueObjects;

public record PlayReturn(int GameId, ReturnCode Code, List<TileOnBoard> TilesPlayed, Rack NewRack, int Points);
