namespace Qwirkle.Domain.ValueObjects;

public record PlayReturn(int GameId, PlayReturnCode Code, HashSet<TileOnBoard> TilesPlayed, Rack NewRack, int Points);