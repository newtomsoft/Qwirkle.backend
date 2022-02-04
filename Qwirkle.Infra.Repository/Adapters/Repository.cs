using System.Threading.Tasks;

namespace Qwirkle.Infra.Repository.Adapters;

public class Repository : IRepository
{
    private readonly DefaultDbContext _dbContext;

    public Repository(DefaultDbContext defaultDbContext) => _dbContext = defaultDbContext;

    public int GetUserId(string userName) => _dbContext.Users.Where(u => u.UserName == userName).Select(u => u.Id).FirstOrDefault();

    public void PutTilesOnBag(int gameId)
    {
        var tilesIds = _dbContext.Tiles.Select(tile => tile.Id);
        foreach (var tileId in tilesIds) _dbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tileId });
        _dbContext.SaveChanges();
    }

    public Player CreatePlayer(int userId, int gameId)
    {
        var playerDao = new PlayerDao { GameId = gameId, UserId = userId, LastTurnSkipped = false };
        _dbContext.Players.Add(playerDao);
        _dbContext.SaveChanges();
        return playerDao.ToPlayer(_dbContext);
    }

    public Game CreateGame(DateTime date)
    {
        var gameDao = new GameDao { CreateDate = date };
        _dbContext.Games.Add(gameDao);
        _dbContext.SaveChanges();
        return gameDao.ToEmptyGame();
    }
    public List<int> GetGamesIdsContainingPlayers() => _dbContext.Players.Select(p => p.GameId).Distinct().ToList();

    public List<int> GetAllUsersId() => _dbContext.Users.Select(u => u.Id).ToList();

    public List<int> GetUserGamesIds(int userId) => _dbContext.Players.Where(p => p.UserId == userId).Select(p => p.GameId).ToList();

    public string GetPlayerNameTurn(int gameId) => _dbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Include(p => p.User).FirstOrDefault()?.User.UserName;

    public int GetPlayerIdToPlay(int gameId) => _dbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Select(p => p.Id).FirstOrDefault();

    public Game GetGame(int gameId)
    {
        var gameOver = _dbContext.Games.Where(g => g.Id == gameId).Select(g => g.GameOver).FirstOrDefault();
        var tilesOnBoardDao = _dbContext.TilesOnBoard.Where(tb => tb.GameId == gameId).ToList();
        var tiles = TilesOnBoardDaoToEntity(tilesOnBoardDao);
        var playersDao = _dbContext.Players.Where(p => p.GameId == gameId).Include(p => p.User).ToList();
        var players = playersDao.Select(playerDao => playerDao.ToPlayer(_dbContext)).ToList();
        var tilesOnBagDao = _dbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
        var bag = new Bag(gameId);
        foreach (var tileOnBagDao in tilesOnBagDao) bag.Tiles.Add(tileOnBagDao.ToTileOnBag());
        return new Game(gameId, Board.From(tiles), players, bag, gameOver);
    }

    public async Task<Game> GetGameAsync(int gameId)
    {
        var gameOver = await _dbContext.Games.Where(g => g.Id == gameId).Select(g => g.GameOver).FirstOrDefaultAsync();
        var tilesOnBoardDao = await _dbContext.TilesOnBoard.Where(tb => tb.GameId == gameId).ToListAsync();
        var tiles = TilesOnBoardDaoToEntity(tilesOnBoardDao);
        var playersDao = await _dbContext.Players.Where(p => p.GameId == gameId).Include(p => p.User).ToListAsync();
        var players = playersDao.Select(playerDao => playerDao.ToPlayer(_dbContext)).ToList();
        var tilesOnBagDao = await _dbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToListAsync();
        var bag = new Bag(gameId);
        foreach (var tileOnBagDao in tilesOnBagDao) bag.Tiles.Add(tileOnBagDao.ToTileOnBag());
        return new Game(gameId, Board.From(tiles), players, bag, gameOver);
    }

    public Player GetPlayer(int playerId) => _dbContext.Players.Where(p => p.Id == playerId).Include(p => p.User).First().ToPlayer(_dbContext);
    public Player GetPlayer(int gameId, int userId) => _dbContext.Players.First(p => p.GameId == gameId && p.UserId == userId).ToPlayer(_dbContext);
    public int GetPlayerId(int gameId, int userId) => _dbContext.Players.FirstOrDefault(p => p.GameId == gameId && p.UserId == userId)?.Id ?? 0;

    public void UpdatePlayer(Player player)
    {
        var playerDao = _dbContext.Players.First(p => p.Id == player.Id);
        playerDao.Points = player.Points;
        playerDao.LastTurnPoints = (byte)player.LastTurnPoints;
        playerDao.GameTurn = player.IsTurn;
        playerDao.LastTurnSkipped = player.LastTurnSkipped;
        playerDao.GamePosition = (byte)player.GamePosition;
        _dbContext.SaveChanges();
    }

    public void ArrangeRack(Player player, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        for (byte i = 0; i < tilesList.Count; i++)
        {
            var tile = _dbContext.TilesOnPlayer.Include(t => t.Tile).First(tp => tp.PlayerId == player.Id && tp.Tile.Color == tilesList[i].Color && tp.Tile.Shape == tilesList[i].Shape);
            tile.RackPosition = i;
        }
        _dbContext.SaveChanges();
    }

    public void TilesFromBagToPlayer(Player player, List<byte> positionsInRack)
    {
        var tilesNumber = positionsInRack.Count;
        var tilesToGiveToPlayer = _dbContext.TilesOnBag.Include(t => t.Tile).Where(t => t.GameId == player.GameId).AsEnumerable().OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
        _dbContext.TilesOnBag.RemoveRange(tilesToGiveToPlayer);
        for (var i = 0; i < tilesToGiveToPlayer.Count; i++)
        {
            var tileOnPlayerDao = tilesToGiveToPlayer[i].ToTileOnPlayerDao(positionsInRack[i], player.Id);
            _dbContext.TilesOnPlayer.Add(tileOnPlayerDao);
            player.Rack.Tiles.Add(tileOnPlayerDao.ToTileOnPlayer());
        }
        _dbContext.SaveChanges();
    }

    public void TilesFromPlayerToBag(Player player, IEnumerable<Tile> tiles)
    {
        var game = _dbContext.Games.Single(g => g.Id == player.GameId);
        game.LastPlayDate = DateTime.UtcNow;

        foreach (var tile in tiles)
        {
            var tileOnPlayerDao = _dbContext.TilesOnPlayer.Include(t => t.Tile).First(t => t.PlayerId == player.Id && t.Tile.Color == tile.Color && t.Tile.Shape == tile.Shape);
            _dbContext.TilesOnPlayer.Remove(tileOnPlayerDao);
            _dbContext.TilesOnBag.Add(tileOnPlayerDao.ToTileOnBagDao(player.GameId));
            _dbContext.SaveChanges();
        }
    }

    public void TilesFromPlayerToBoard(int gameId, int playerId, IEnumerable<TileOnBoard> tilesOnBoard)
    {
        var game = _dbContext.Games.Single(g => g.Id == gameId);
        game.LastPlayDate = DateTime.UtcNow;
        var tiles = tilesOnBoard.ToList();
        foreach (var tile in tiles) _dbContext.TilesOnBoard.Add(_dbContext.TilesOnPlayer.Include(t => t.Tile).First(tp => tp.PlayerId == playerId && tp.Tile.Color == tile.Color && tp.Tile.Shape == tile.Shape).ToTileOnBoardDao(tile.Coordinates));
        foreach (var tile in tiles) _dbContext.TilesOnPlayer.Remove(_dbContext.TilesOnPlayer.Include(t => t.Tile).First(tp => tp.PlayerId == playerId && tp.Tile.Color == tile.Color && tp.Tile.Shape == tile.Shape));
        _dbContext.SaveChanges();
    }

    public void SetPlayerTurn(int playerId)
    {
        var player = _dbContext.Players.Single(p => p.Id == playerId);
        player.GameTurn = true;
        _dbContext.SaveChanges();
    }

    public void SetGameOver(int gameId)
    {
        foreach (var player in _dbContext.Players.Where(p => p.GameId == gameId)) player.GameTurn = false;
        _dbContext.Games.Single(g => g.Id == gameId).GameOver = true;
        _dbContext.SaveChanges();
    }

    public List<int> GetLeadersPlayersId(int gameId)
    {
        var players = _dbContext.Players.Where(player => player.GameId == gameId).ToList();
        var maxPoints = players.Max(player => player.Points);
        return players.Where(p => p.Points == maxPoints).Select(p => p.Id).ToList();
    }

    public bool IsGameOver(int gameId) => _dbContext.Games.Any(g => g.Id == gameId && g.GameOver);

    private List<TileOnBoard> TilesOnBoardDaoToEntity(IReadOnlyCollection<TileOnBoardDao> tilesOnBoard)
    {
        var tilesDao = _dbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
        return (from tileDao in tilesDao let tileOnBoardDao = tilesOnBoard.First(tb => tb.TileId == tileDao.Id) select new TileOnBoard(tileDao.Color, tileDao.Shape, new Coordinates(tileOnBoardDao.PositionX, tileOnBoardDao.PositionY))).ToList();
    }

    public bool AddBookmarkedOpponent(int userId, string opponentName)
    {
        var user = _dbContext.Users.Include(u => u.BookmarkedOpponents).First(u => u.Id == userId);
        var opponent = _dbContext.Users.Include(u => u.BookmarkedBy).First(u => u.UserName == opponentName);
        user.BookmarkedOpponents.Add(opponent);
        opponent.BookmarkedBy.Add(user);
        return _dbContext.SaveChanges() == 1;
    }

    public bool RemoveBookmarkedOpponent(int userId, string opponentName)
    {
        var user = _dbContext.Users.Include(u => u.BookmarkedOpponents).First(u => u.Id == userId);
        var opponent = _dbContext.Users.Include(u => u.BookmarkedBy).First(u => u.UserName == opponentName);
        user.BookmarkedOpponents.Remove(opponent);
        opponent.BookmarkedBy.Remove(user);
        return _dbContext.SaveChanges() == 1;
    }

    public HashSet<string> GetBookmarkedOpponentsNames(int userId)
    {
        var opponents = _dbContext.Users.Include(u => u.BookmarkedOpponents).First(u => u.Id == userId).BookmarkedOpponents;
        return opponents.Select(o => o.UserName).ToHashSet();
    }
}

