namespace Qwirkle.Infra.Repository.DaoExtensionMethods;

public static class DaoExtensionsMethods
{
    public static Game ToEmptyGame(this GameDao gameDao) => new(gameDao.Id, Board.Empty, new List<Player>(), gameDao.GameOver);

    public static Tile ToTile(this TileDao tileDao) => new(tileDao.Color, tileDao.Shape);
    public static TileOnRack ToTileOnRack(this TileDao tileDao, RackPosition position) => new(position, tileDao.Color, tileDao.Shape);

    public static TileOnBag ToTileOnBag(this TileOnBagDao tileOnBagDao) => new(tileOnBagDao.Tile.Color, tileOnBagDao.Tile.Shape);

    public static TileOnRack ToTileOnRack(this TileOnRackDao tileOnRackDao) => new(tileOnRackDao.RackPosition, tileOnRackDao.Tile.Color, tileOnRackDao.Tile.Shape);

    public static TileOnBoardDao ToTileOnBoardDao(this TileOnRackDao tileOnRackDao, Coordinate coordinate) => new() { PositionX = coordinate.X, PositionY = coordinate.Y, TileId = tileOnRackDao.TileId, GameId = tileOnRackDao.Player.GameId };

    public static Player ToPlayer(this PlayerDao playerDao, DefaultDbContext dbContext)
    {
        playerDao.Tiles ??= dbContext.TilesOnRack.Where(tp => tp.PlayerId == playerDao.Id).Include(t => t.Tile).ToList();
        playerDao.User ??= dbContext.Users.First(u => u.Id == playerDao.UserId);
        var rack = Rack.From(playerDao.Tiles.Select(tileOnPlayerDao => tileOnPlayerDao.ToTileOnRack()).ToList());
        return new Player(playerDao.Id, playerDao.User.Id, playerDao.GameId, playerDao.User.UserName, playerDao.GamePosition, playerDao.Points, playerDao.LastTurnPoints, rack, playerDao.GameTurn, playerDao.LastTurnSkipped, playerDao.User.ToUser());
    }

    public static User ToUser(this UserDao userDao) => new(userDao.UserName, userDao.Email, userDao.FirstName, userDao.LastName, userDao.Help, userDao.Points, userDao.GamesPlayed, userDao.GamesWon);

    public static TileOnRackDao ToTileOnRackDao(this TileOnBagDao tileOnBagDao, byte rackPosition, int playerId) => new(tileOnBagDao, rackPosition, playerId);

}

