namespace Qwirkle.Infra.Repository.Dao;

[Table("Tile")]
public class TileDao
{
    public int Id { get; set; }
    public TileShape Shape { get; set; }
    public TileColor Color { get; set; }
}
