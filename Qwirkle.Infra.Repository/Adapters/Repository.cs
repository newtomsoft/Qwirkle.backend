﻿namespace Qwirkle.Infra.Repository.Adapters;
public class Repository : IRepository
{
    private DefaultDbContext DbContext { get; }
    private const int TotalTiles = 108;

    public Repository(DefaultDbContext defaultDbContext) => DbContext = defaultDbContext;

    public int GetUserId(int playerId) => DbContext.Players.First(p => p.Id == playerId).UserId;
    public int GetUserId(string userName) => DbContext.Users.Where(u => u.UserName == userName).Select(u => u.Id).FirstOrDefault();
    public void CreateTiles(int gameId)
    {
        AddAllTilesInDataBaseIfNotPresent();
        AddAllTilesOnBag(gameId);
    }

    public Player CreatePlayer(int userId, int gameId)
    {
        var playerDao = new PlayerDao { GameId = gameId, UserId = userId, LastTurnSkipped = false };
        DbContext.Players.Add(playerDao);
        DbContext.SaveChanges();
        return playerDao.ToPlayer(DbContext);
    }

    public Game CreateGame(DateTime date)
    {
        var gameDao = new GameDao { CreateDate = date };
        DbContext.Games.Add(gameDao);
        DbContext.SaveChanges();
        return gameDao.ToEmptyGame();
    }
    public List<int> GetGamesIdsContainingPlayers() => DbContext.Players.Select(p => p.GameId).Distinct().ToList();

    public List<int> GetAllUsersId() => DbContext.Users.Select(u => u.Id).ToList();

    public List<int> GetUserGamesIds(int userId) => DbContext.Players.Where(p => p.UserId == userId).Select(p => p.GameId).ToList();

    public string GetPlayerNameTurn(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Include(p => p.User).FirstOrDefault()?.User.UserName;

    public int GetPlayerIdToPlay(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Select(p => p.Id).FirstOrDefault();

    public Tile GetTile(TileColor color, TileShape shape) => DbContext.Tiles.First(t => t.Color == color && t.Shape == shape).ToTile();

    public TileOnPlayer GetTileOnPlayerById(int playerId, int tileId) => DbContext.TilesOnPlayer.Single(t => t.PlayerId == playerId && t.TileId == tileId).ToTileOnPlayer();

    public Game GetGame(int gameId)
    {
        var gameOver = DbContext.Games.Where(g => g.Id == gameId).Select(g => g.GameOver).FirstOrDefault();
        var tilesOnBoardDao = DbContext.TilesOnBoard.Where(tb => tb.GameId == gameId).ToList();
        var tiles = TilesOnBoardDaoToEntity(tilesOnBoardDao);
        var playersDao = DbContext.Players.Where(p => p.GameId == gameId).Include(p => p.User).ToList();
        var players = playersDao.Select(playerDao => playerDao.ToPlayer(DbContext)).ToList();
        var tilesOnBagDao = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
        var bag = new Bag(gameId);
        foreach (var tileOnBagDao in tilesOnBagDao) bag.Tiles.Add(tileOnBagDao.ToTileOnBag());
        return new Game(gameId, Board.From(tiles), players, bag, gameOver);
    }

    public Player GetPlayer(int playerId) => DbContext.Players.Where(p => p.Id == playerId).Include(p => p.User).First().ToPlayer(DbContext);
    public Player GetPlayer(int gameId, int userId) => DbContext.Players.First(p => p.GameId == gameId && p.UserId == userId).ToPlayer(DbContext);
    public int GetPlayerId(int gameId, int userId) => DbContext.Players.FirstOrDefault(p => p.GameId == gameId && p.UserId == userId)?.Id ?? 0;

    public void UpdatePlayer(Player player)
    {
        var playerDao = DbContext.Players.First(p => p.Id == player.Id);
        playerDao.Points = player.Points;
        playerDao.LastTurnPoints = (byte)player.LastTurnPoints;
        playerDao.GameTurn = player.IsTurn;
        playerDao.LastTurnSkipped = player.LastTurnSkipped;
        playerDao.GamePosition = (byte)player.GamePosition;
        DbContext.SaveChanges();
    }

    public void ArrangeRack(Player player, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        for (byte i = 0; i < tiles.Count(); i++)
        {
            var tile = DbContext.TilesOnPlayer.Include(t => t.Tile).First(tp => tp.PlayerId == player.Id && tp.Tile.Color == tilesList[i].Color && tp.Tile.Shape == tilesList[i].Shape);
            tile.RackPosition = i;
        }
        DbContext.SaveChanges();
    }


    public void TilesFromBagToPlayer(Player player, List<byte> positionsInRack)
    {
        var tilesNumber = positionsInRack.Count;
        var tilesToGiveToPlayer = DbContext.TilesOnBag.Include(t => t.Tile).Where(t => t.GameId == player.GameId).AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
        DbContext.TilesOnBag.RemoveRange(tilesToGiveToPlayer);
        for (var i = 0; i < tilesToGiveToPlayer.Count; i++)
        {
            var tileOnPlayerDao = tilesToGiveToPlayer[i].ToTileOnPlayerDao(positionsInRack[i], player.Id);
            DbContext.TilesOnPlayer.Add(tileOnPlayerDao);
            player.Rack.Tiles.Add(tileOnPlayerDao.ToTileOnPlayer());
        }
        DbContext.SaveChanges();
    }

    public void TilesFromPlayerToBag(Player player, IEnumerable<Tile> tiles)
    {
        var game = DbContext.Games.Single(g => g.Id == player.GameId);
        game.LastPlayDate = DateTime.UtcNow;
        var tilesOnPlayerDao = tiles.Select(tile => DbContext.TilesOnPlayer.Include(t => t.Tile).First(t => t.PlayerId == player.Id && t.Tile.Color == tile.Color && t.Tile.Shape == tile.Shape)).ToList();
        DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayerDao);
        foreach (var tileOnPlayerDao in tilesOnPlayerDao) DbContext.TilesOnBag.Add(tileOnPlayerDao.ToTileOnBagDao(player.GameId));
        DbContext.SaveChanges();
    }

    public void TilesFromPlayerToBoard(int gameId, int playerId, IEnumerable<TileOnBoard> tilesOnBoard)
    {
        var game = DbContext.Games.Single(g => g.Id == gameId);
        game.LastPlayDate = DateTime.UtcNow;
        var tiles = tilesOnBoard.ToList();
        foreach (var tile in tiles) DbContext.TilesOnBoard.Add(DbContext.TilesOnPlayer.Include(t => t.Tile).First(tp => tp.PlayerId == playerId && tp.Tile.Color == tile.Color && tp.Tile.Shape == tile.Shape).ToTileOnBoardDao(tile.Coordinates));
        foreach (var tile in tiles) DbContext.TilesOnPlayer.Remove(DbContext.TilesOnPlayer.Include(t => t.Tile).First(tp => tp.PlayerId == playerId && tp.Tile.Color == tile.Color && tp.Tile.Shape == tile.Shape));
        DbContext.SaveChanges();
    }


    public void SetPlayerTurn(int playerId)
    {
        var player = DbContext.Players.Single(p => p.Id == playerId);
        player.GameTurn = true;
        DbContext.SaveChanges();
    }

    public void SetGameOver(int gameId)
    {
        foreach (var player in DbContext.Players.Where(p => p.GameId == gameId)) player.GameTurn = false;
        DbContext.Games.Single(g => g.Id == gameId).GameOver = true;
        DbContext.SaveChanges();
    }



    public List<int> GetLeadersPlayersId(int gameId)
    {
        var players = DbContext.Players.Where(player => player.GameId == gameId).ToList();
        var maxPoints = players.Max(player => player.Points);
        return players.Where(p => p.Points == maxPoints).Select(p => p.Id).ToList();
    }


    public bool IsGameOver(int gameId) => DbContext.Games.Any(g => g.Id == gameId && g.GameOver);

    private void AddAllTilesInDataBaseIfNotPresent()
    {
        if (DbContext.Tiles.Count() == TotalTiles) return;
        const int numberOfSameTile = 3;
        for (var i = 0; i < numberOfSameTile; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    DbContext.Tiles.Add(new TileDao { Color = color, Shape = shape });
        DbContext.SaveChanges();
    }

    private void AddAllTilesOnBag(int gameId)
    {
        var tilesIds = DbContext.Tiles.Select(tile => tile.Id).ToList();
        for (var i = 0; i < TotalTiles; i++)
            DbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tilesIds[i] });
        DbContext.SaveChanges();
    }

    private List<TileOnBoard> TilesOnBoardDaoToEntity(IReadOnlyCollection<TileOnBoardDao> tilesOnBoard)
    {
        var tilesDao = DbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
        return (from tileDao in tilesDao let tileOnBoardDao = tilesOnBoard.Single(tb => tb.TileId == tileDao.Id) select new TileOnBoard(tileDao.Color, tileDao.Shape, new Coordinates(tileOnBoardDao.PositionX, tileOnBoardDao.PositionY))).ToList();
    }
}

