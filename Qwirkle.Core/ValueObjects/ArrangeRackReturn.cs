namespace Qwirkle.Core.ValueObjects;

public record PlayReturn(int GameId, PlayReturnCode Code, List<TileOnBoard> TilesPlayed, Rack NewRack, int Points);

