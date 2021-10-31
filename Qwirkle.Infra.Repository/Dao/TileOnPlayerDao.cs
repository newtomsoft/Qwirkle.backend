namespace Qwirkle.Infra.Repository.Dao;

[Table("TileOnPlayer")]
public class TileOnPlayerDao
{
    public int Id { get; set; }
    public int TileId { get; set; }
    public int PlayerId { get; set; }
    public byte RackPosition { get; set; }

    public virtual TileDao Tile { get; set; }
    public virtual PlayerDao Player { get; set; }


    public TileOnPlayerDao() { }

    public TileOnPlayerDao(TileOnBagDao tb, byte rackPosition, int playerId)
    {
        TileId = tb.TileId;
        PlayerId = playerId;
        RackPosition = rackPosition;
    }
}
