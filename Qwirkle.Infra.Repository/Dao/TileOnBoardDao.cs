namespace Qwirkle.Infra.Repository.Dao;

[Table("TileOnBoard")]
public class TileOnBoardDao
{
    public int Id { get; set; }
    public int TileId { get; set; }
    public int GameId { get; set; }
    public sbyte PositionX { get; set; }
    public sbyte PositionY { get; set; }

    public virtual TileDao Tile { get; set; }
    public virtual GameDao Game { get; set; }
}
