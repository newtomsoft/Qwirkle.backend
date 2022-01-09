namespace Qwirkle.Infra.Repository.Dao;

[Table("Game")]
public class GameDao
{
    public int Id { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastPlayDate { get; set; }
    public bool GameOver { get; set; }

    public virtual List<PlayerDao> Players { get; set; }
    public virtual List<TileOnBoardDao> TilesOnBoard { get; set; }
    public virtual List<TileOnBagDao> TilesOnBag { get; set; }
}
