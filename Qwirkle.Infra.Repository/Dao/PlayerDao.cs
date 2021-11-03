namespace Qwirkle.Infra.Repository.Dao;

[Table("Player")]
public class PlayerDao
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int UserId { get; set; }
    public byte Points { get; set; }
    public byte LastTurnPoints { get; set; }
    public bool GameTurn { get; set; }
    public byte GamePosition { get; set; }
    public bool LastTurnSkipped { get; set; }

    public virtual GameDao Game { get; set; }
    public virtual UserDao User { get; set; }
    public virtual List<TileOnPlayerDao> Tiles { get; set; }

    public Player ToPlayer(DefaultDbContext dbContext)
    {
        if (Tiles is null) Tiles = dbContext.TilesOnPlayer.Where(tp => tp.PlayerId == Id).Include(t => t.Tile).ToList();
        var tilesOnPlayer = new List<TileOnPlayer>();
        Tiles.ForEach(tileOnPlayerDao => tilesOnPlayer.Add(tileOnPlayerDao.ToTileOnPlayer()));
        var player = new Player(Id, GameId, User?.UserName, GamePosition, Points, LastTurnPoints, tilesOnPlayer, GameTurn, LastTurnSkipped);
        return player;
    }
}
