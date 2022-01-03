namespace Qwirkle.Infra.Repository.DomainExtensionMethods;

public static class DomainExtensionsMethods
{
    public static TileOnBoardDao ToTileOnBoardDao(this TileOnBoard tile, int gameId) => new() { TileId = 0, GameId = gameId, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y };
    public static TileOnBagDao ToTileOnBagDao(this TileOnPlayerDao tileOnPlayer, int gameId) => new() { TileId = tileOnPlayer.TileId, GameId = gameId };
    public static UserDao ToUserDao(this User user) => new() { UserName = user.Pseudo, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, };
}

