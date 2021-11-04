namespace Qwirkle.Infra.Repository.Adapters;
public class Repository : IRepository
{
    private DefaultDbContext DbContext { get; }

    private const int TotalTiles = 108;

    public Repository(DefaultDbContext defaultDbContext)
    {
        DbContext = defaultDbContext;
    }

    public void CreateTiles(int gameId)
    {
        AddAllTilesInDataBase();
        var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
        for (int i = 0; i < TotalTiles; i++)
            DbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tilesIds[i] });

        DbContext.SaveChanges();
    }

    public Player CreatePlayer(int userId, int gameId)
    {
        if (!DbContext.Users.Any()) CreateFirstUsersCreatorOnlineGame();
        var playerDao = new PlayerDao { GameId = gameId, UserId = userId, LastTurnSkipped = false };
        DbContext.Players.Add(playerDao);
        DbContext.SaveChanges();
        return playerDao.ToPlayer(DbContext);
    }

    public Game CreateGame(DateTime date)
    {
        var gameDao = new GameDao { CreatDate = date };
        DbContext.Games.Add(gameDao);
        DbContext.SaveChanges();
        return gameDao.ToNotFillGame();
    }
    public List<int> GetGamesIdsContainingPlayers() => DbContext.Players.Select(p => p.GameId).Distinct().ToList();

    public List<int> GetUsersId() => DbContext.Users.Select(u => u.Id).ToList();

    public List<int> GetUserGames(int userId) => DbContext.Players.Where(p => p.UserId == userId).Select(p => p.GameId).ToList();

    public List<string> GetPlayersNames(int gameId)
    {
        var listName = new List<string>();
        var players = DbContext.Players.Where(p => p.GameId == gameId).OrderBy(u => u.UserId).ToList();
        players.ForEach(player => listName.Add(DbContext.Users.Where(u => u.Id == player.UserId).OrderBy(u => u.Id).Select(u => u.UserName).FirstOrDefault()));
        return listName;
    }

    public string GetPlayerNameTurn(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Include(p => p.User).FirstOrDefault()?.User.UserName;

    public int GetPlayerIdToPlay(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Select(p => p.Id).FirstOrDefault();

    public Tile GetTileById(int tileId) => DbContext.Tiles.Single(t => t.Id == tileId).ToTile();

    public TileOnPlayer GetTileOnPlayerById(int playerId, int tileId) => DbContext.TilesOnPlayer.Single(t => t.PlayerId == playerId && t.TileId == tileId).ToTileOnPlayer();

    public Game GetGame(int gameId)
    {
        var gameOver = DbContext.Games.Where(g => g.Id == gameId).Select(g => g.GameOver).FirstOrDefault();
        var tilesOnBoardDao = DbContext.TilesOnBoard.Where(tb => tb.GameId == gameId).ToList();
        var tiles = TilesOnBoardDaoToEntity(tilesOnBoardDao);
        var playersDao = DbContext.Players.Where(p => p.GameId == gameId).Include(p => p.User).ToList();
        var players = new List<Player>();
        playersDao.ForEach(playerDao => players.Add(playerDao.ToPlayer(DbContext)));
        var tilesOnBagDao = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
        var bag = new Bag(gameId);
        tilesOnBagDao.ForEach(tileOnBagDao => bag.Tiles.Add(tileOnBagDao.ToTileOnBag()));
        return new Game(gameId, tiles, players, gameOver, bag);
    }

    public Player GetPlayer(int playerId) => DbContext.Players.Where(p => p.Id == playerId).Include(p => p.User).First().ToPlayer(DbContext);
    public Player GetPlayer(int gameId, int userId) => DbContext.Players.First(p => p.GameId == gameId && p.UserId == userId).ToPlayer(DbContext);

    public void UpdatePlayer(Player player)
    {
        DbContext.Players.Update(PlayerToPlayerDaoWithoutTile(player));
        DbContext.SaveChanges();
    }

    public void ArrangeRack(Player player, List<TileOnPlayer> tilesToArrange)
    {
        for (byte i = 0; i < tilesToArrange.Count; i++)
        {
            var tile = DbContext.TilesOnPlayer.Single(tp => tp.PlayerId == player.Id && tp.TileId == tilesToArrange[i].Id);
            tile.RackPosition = i;
        }
        DbContext.SaveChanges();
    }

    public void TilesFromBagToPlayer(Player player, List<byte> positionsInRack)
    {
        int tilesNumber = positionsInRack.Count;
        var tilesToGiveToPlayer = DbContext.TilesOnBag.Where(t => t.GameId == player.GameId).ToList().OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
        DbContext.TilesOnBag.RemoveRange(tilesToGiveToPlayer);
        for (int i = 0; i < tilesToGiveToPlayer.Count; i++)
            DbContext.TilesOnPlayer.Add(new TileOnPlayerDao(tilesToGiveToPlayer[i], positionsInRack[i], player.Id));
        DbContext.SaveChanges();
    }

    public void TilesFromPlayerToBag(Player player, List<TileOnPlayer> tiles)
    {
        var game = DbContext.Games.Single(g => g.Id == player.GameId);
        game.LastPlayDate = DateTime.UtcNow;
        var tilesOnPlayerDao = DbContext.TilesOnPlayer.Where(t => t.PlayerId == player.Id && tiles.Select(tile => tile.Id).Contains(t.TileId)).ToList();
        DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayerDao);
        tilesOnPlayerDao.ForEach(tileOnPlayerDao => DbContext.TilesOnBag.Add(tileOnPlayerDao.ToTileOnBagDao(player.GameId)));
        DbContext.SaveChanges();
    }

    public void TilesFromPlayerToBoard(int gameId, int playerId, List<TileOnBoard> tiles)
    {
        var game = DbContext.Games.Single(g => g.Id == gameId);
        game.LastPlayDate = DateTime.UtcNow;
        tiles.ForEach(tile => DbContext.TilesOnBoard.Add(tile.ToTileOnBoardDao(gameId)));
        tiles.ForEach(tile => DbContext.TilesOnPlayer.Remove(DbContext.TilesOnPlayer.Single(tp => tp.TileId == tile.Id && tp.PlayerId == playerId)));
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

    public List<int> GetLeadersPlayersId(int gameId) => DbContext.Players.Where(player => player.GameId == gameId).OrderByDescending(player => player.Points).GroupBy(player => player.Points).First().Select(player => player.Id).ToList();

    public bool IsGameOver(int gameId) => DbContext.Games.Any(g => g.Id == gameId && g.GameOver);

    private void AddAllTilesInDataBase()
    {
        if (DbContext.Tiles.Count() == TotalTiles) return;

        const int numberOfSameTile = 3;
        for (var i = 0; i < numberOfSameTile; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    DbContext.Tiles.Add(new TileDao { Color = color, Shape = shape });

        DbContext.SaveChanges();
    }

    private void CreateFirstUsersCreatorOnlineGame()
    {
        var tomJc = new List<UserDao>
        {
            new UserDao { Id=1, UserName = "Tom", FirstName = "Thomas", LastName = "Vuille" },
            new UserDao { Id=2, UserName = "JC", FirstName = "Jean Charles", LastName = "Gouleau" },
        };
        DbContext.Users.AddRange(tomJc);
        DbContext.SaveChanges();
    }

    private List<TileOnBoard> TilesOnBoardDaoToEntity(List<TileOnBoardDao> tilesOnBoard)
    {
        var tiles = new List<TileOnBoard>();
        var tilesDao = DbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
        foreach (var tileDao in tilesDao)
        {
            var tileOnGame = tilesOnBoard.Single(tb => tb.TileId == tileDao.Id);
            tiles.Add(new TileOnBoard(tileDao.Id, tileDao.Color, tileDao.Shape, new CoordinatesInGame(tileOnGame.PositionX, tileOnGame.PositionY)));
        }
        return tiles;
    }

    private PlayerDao PlayerToPlayerDaoWithoutTile(Player player)
    {
        var playerDao = DbContext.Players.Single(gp => gp.Id == player.Id);
        playerDao.Points = (byte)player.Points;
        playerDao.LastTurnPoints = (byte)player.LastTurnPoints;
        playerDao.GameTurn = player.IsTurn;
        playerDao.GamePosition = (byte)player.GamePosition;
        playerDao.LastTurnSkipped = player.LastTurnSkipped;
        return playerDao;
    }
}

