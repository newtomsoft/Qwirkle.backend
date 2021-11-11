﻿namespace Qwirkle.Infra.Repository.Dao;

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
        Tiles ??= dbContext.TilesOnPlayer.Where(tp => tp.PlayerId == Id).Include(t => t.Tile).ToList();
        User??= dbContext.Users.First(u => u.Id == UserId);
        var tilesOnPlayer = Tiles.Select(tileOnPlayerDao => tileOnPlayerDao.ToTileOnPlayer()).ToList();
        var player = new Player(Id, User.Id, GameId, User.UserName, GamePosition, Points, LastTurnPoints, tilesOnPlayer, GameTurn, LastTurnSkipped);
        return player;
    }
}
