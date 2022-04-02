namespace Qwirkle.Infra.Repository.Dao;

[Table("Player")]
public class PlayerDao
{
    public int Id { get; init; }
    public int GameId { get; init; }
    public int UserId { get; init; }
    public int Points { get; set; }
    public byte LastTurnPoints { get; set; }
    public bool GameTurn { get; set; }
    public byte GamePosition { get; set; }
    public bool LastTurnSkipped { get; set; }

    public virtual GameDao Game { get; set; }
    public virtual UserDao User { get; set; }
    public virtual List<TileOnRackDao> Tiles { get; set; }
}
