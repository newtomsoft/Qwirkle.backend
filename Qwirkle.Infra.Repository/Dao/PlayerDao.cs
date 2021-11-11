namespace Qwirkle.Infra.Repository.Dao;

[Table("Player")]
public class PlayerDao
{
    public int Id { get; init; }
    public int GameId { get; init; }
    public int UserId { get; init; }
    public byte Points { get; set; }
    public byte LastTurnPoints { get; set; }
    public bool GameTurn { get; set; }
    public byte GamePosition { get; init; }
    public bool LastTurnSkipped { get; set; }

    public virtual GameDao Game { get; set; }
    public virtual UserDao User { get; set; }
    public virtual List<TileOnPlayerDao> Tiles { get; set; }

    public Player ToPlayer(DefaultDbContext dbContext)
    {
        Tiles ??= dbContext.TilesOnPlayer.Where(tp => tp.PlayerId == Id).Include(t => t.Tile).ToList();
        User??= dbContext.Users.First(u => u.Id == UserId);
        var rack = new Rack(Tiles.Select(tileOnPlayerDao => tileOnPlayerDao.ToTileOnPlayer()).ToList());
        return new Player(Id, User.Id, GameId, User.UserName, GamePosition, Points, LastTurnPoints, rack, GameTurn, LastTurnSkipped);
    }
}
