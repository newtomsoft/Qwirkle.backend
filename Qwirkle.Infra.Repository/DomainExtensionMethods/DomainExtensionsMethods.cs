namespace Qwirkle.Infra.Repository.DomainExtensionMethods;

public static class DomainExtensionsMethods
{
    public static TileOnBoardDao ToTileOnBoardDao(this TileOnBoard tile, int gameId) => new() { TileId = 0, GameId = gameId, PositionX = tile.Coordinate.X, PositionY = tile.Coordinate.Y };
    public static TileOnBagDao ToTileOnBagDao(this TileOnRackDao tileOnRack, int gameId) => new() { TileId = tileOnRack.TileId, GameId = gameId };
    public static UserDao ToUserDao(this User user) => new() { UserName = user.Pseudo, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, };
}

