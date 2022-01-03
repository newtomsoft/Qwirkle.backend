namespace Qwirkle.Infra.Repository.Dao;

[Table("TileOnBag")]
public class TileOnBagDao
{
    public int Id { get; set; }
    public int TileId { get; set; }
    public int GameId { get; set; }

    public virtual TileDao Tile { get; set; }
    public virtual GameDao Game { get; set; }
}
