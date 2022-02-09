namespace Qwirkle.Test;

public class PlayTilesShould
{
    #region private

    private DefaultDbContext _dbContext = null!;
    private Repository _repository = null!;
    private InfoService _infoService = null!;
    private CoreService _coreService = null!;

    private const int GameId = 7;
    private const int User71 = 71;
    private const int User21 = 21;
    private const int User3 = 3;
    private const int User14 = 14;
    private const int Player9 = 9;
    private const int Player3 = 3;
    private const int Player8 = 8;
    private const int Player14 = 14;

    private void InitDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DefaultDbContext(contextOptions);
        InitializeData();
    }

    private void InitializeData()
    {
        AddAllTiles();
        AddUsers();
        AddGames();
        Add2Players();
        AddTilesOnPlayers();
        AddTilesOnBag();
    }

    private void InitTest()
    {
        InitDbContext();
        _repository = new Repository(_dbContext);
        _infoService = new InfoService(_repository, null, new Logger<InfoService>(new LoggerFactory()));
        _coreService = new CoreService(_repository, null, _infoService, null, new Logger<CoreService>(new LoggerFactory()));
    }

    private void AddAllTiles()
    {
        const int numberOfSameTile = 3;
        var id = 0;
        for (var i = 0; i < numberOfSameTile; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    _dbContext.Tiles.Add(new TileDao { Id = ++id, Color = color, Shape = shape });

        _dbContext.SaveChanges();
    }

    private void AddGames()
    {
        _dbContext.Games.Add(new GameDao { Id = GameId, });
        _dbContext.SaveChanges();
    }

    private void AddUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User71 });
        _dbContext.Users.Add(new UserDao { Id = User21 });
        _dbContext.Users.Add(new UserDao { Id = User3 });
        _dbContext.Users.Add(new UserDao { Id = User14 });
        _dbContext.SaveChanges();
    }

    private void Add2Players()
    {
        _dbContext.Players.Add(new PlayerDao { Id = Player9, UserId = User71, GameId = GameId, GamePosition = 0, GameTurn = true });
        _dbContext.Players.Add(new PlayerDao { Id = Player3, UserId = User21, GameId = GameId, GamePosition = 1, GameTurn = false });
        _dbContext.SaveChanges();
    }

    private void AddTilesOnPlayers()
    {
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 1, PlayerId = Player9, TileId = 1 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 2, PlayerId = Player9, TileId = 2 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 3, PlayerId = Player9, TileId = 3 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 4, PlayerId = Player9, TileId = 4 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 5, PlayerId = Player9, TileId = 5 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 6, PlayerId = Player9, TileId = 6 });

        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 11, PlayerId = Player3, TileId = 7 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 12, PlayerId = Player3, TileId = 8 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 13, PlayerId = Player3, TileId = 9 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 14, PlayerId = Player3, TileId = 10 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 15, PlayerId = Player3, TileId = 11 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 16, PlayerId = Player3, TileId = 12 });

        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 21, PlayerId = Player8, TileId = 13 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 22, PlayerId = Player8, TileId = 14 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 23, PlayerId = Player8, TileId = 15 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 24, PlayerId = Player8, TileId = 16 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 25, PlayerId = Player8, TileId = 17 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 26, PlayerId = Player8, TileId = 18 });

        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 31, PlayerId = Player14, TileId = 19 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 32, PlayerId = Player14, TileId = 20 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 33, PlayerId = Player14, TileId = 21 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 34, PlayerId = Player14, TileId = 22 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 35, PlayerId = Player14, TileId = 23 });
        _dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 36, PlayerId = Player14, TileId = 24 });

        _dbContext.SaveChanges();
    }

    private void AddTilesOnBag()
    {
        for (var i = 1; i <= CoreService.TotalTilesNumber; i++)
            _dbContext.TilesOnBag.Add(new TileOnBagDao { Id = 100 + i, GameId = GameId, TileId = i });
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void ReturnNotPlayerTurnWhenItsNotTurnPlayer()
    {
        InitTest();
        var tilesToPlay = new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, Coordinates.From(-4, 4)), new(TileColor.Blue, TileShape.Clover, Coordinates.From(-4, 3)), new(TileColor.Blue, TileShape.Diamond, Coordinates.From(-4, 2)) };
        var playReturn = _coreService.TryPlayTiles(Player3, tilesToPlay);
        playReturn.Code.ShouldBe(ReturnCode.NotPlayerTurn);
        playReturn.Move.Points.ShouldBe(0);
    }

    [Fact]
    public void ReturnPlayerDoesntHaveThisTileAfter1PlayerHavePlayedNotHisTiles()
    {
        InitTest();
        var tilesToPlay = new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, Coordinates.From(-3, 4)) };
        var playReturn = _coreService.TryPlayTiles(Player9, tilesToPlay);
        playReturn.Code.ShouldBe(ReturnCode.PlayerDoesntHaveThisTile);
        playReturn.Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return12After1PlayerHavePlayedHisFullRackForFirstMove()
    {
        InitTest();
        var tilesToPlay = new List<TileOnBoard>
            { new(TileColor.Green, TileShape.Circle, Coordinates.From(-4, 4)),
              new(TileColor.Green, TileShape.Square, Coordinates.From(-4, 3)),
              new(TileColor.Green, TileShape.Diamond, Coordinates.From(-4, 2)),
              new(TileColor.Green, TileShape.FourPointStar, Coordinates.From(-4, 1)),
              new(TileColor.Green, TileShape.EightPointStar, Coordinates.From(-4, 0)),
              new(TileColor.Green, TileShape.Clover, Coordinates.From(-4, -1))
            };
        var playReturn = _coreService.TryPlayTiles(Player9, tilesToPlay);
        playReturn.Code.ShouldBe(ReturnCode.Ok);
        playReturn.Move.Points.ShouldBe(6 + 6);
    }

    [Fact]
    public void ReturnNotMostPointsMoveAfterFirstPlayerHavePlayedNotMostPointsForFirstMove()
    {
        InitTest();
        var tilesToPlay = new List<TileOnBoard>
        { new(TileColor.Green, TileShape.Circle, Coordinates.From(-4, 4)),
            new(TileColor.Green, TileShape.Square, Coordinates.From(-4, 3)),
            new(TileColor.Green, TileShape.Diamond, Coordinates.From(-4, 2)),
            new(TileColor.Green, TileShape.FourPointStar, Coordinates.From(-4, 1)),
            new(TileColor.Green, TileShape.EightPointStar, Coordinates.From(-4, 0)),
        };
        var playReturn = _coreService.TryPlayTiles(Player9, tilesToPlay);
        playReturn.Code.ShouldBe(ReturnCode.NotMostPointsMove);
    }

    [Fact]
    public void ReturnOkAnd5PointsAfter2PlayersHavePlayed()
    {
        InitTest();
        InitBoard();

        var tilesToPlay = new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, Coordinates.From(-3, 4)), new(TileColor.Green, TileShape.Square, Coordinates.From(-3, 5)), new(TileColor.Green, TileShape.Diamond, Coordinates.From(-3, 6)) };
        _coreService.TryPlayTiles(Player9, tilesToPlay).Move.Points.ShouldBe(5);

        void InitBoard()
        {
            _dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 74, PositionX = -4, PositionY = 4 });
            _dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 75, PositionX = -4, PositionY = 3 });
            _dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 76, PositionX = -4, PositionY = 2 });
            _dbContext.SaveChanges();
        }
    }

    [Fact]
    public void ReturnNotFreeWhenCoordinateOnBoardIsNotFree()
    {
        InitTest();
        InitBoard();

        var tilesToPlay = new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, Coordinates.From(5, 7)) };
        _coreService.TryPlayTiles(Player9, tilesToPlay).Code.ShouldBe(ReturnCode.NotFree);

        void InitBoard()
        {
            _dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 69, PositionX = 5, PositionY = 7 });
            _dbContext.SaveChanges();
        }
    }

    [Fact]
    public void PersistGoodAfter2PlayersMoves()
    {
        InitTest();
        List<TileOnBoard> tilesPlayedOrdered = new();

        var tilesToPlay = new List<TileOnBoard>
        {
            new(TileColor.Green, TileShape.Circle, Coordinates.From(-4, 4)),
            new(TileColor.Green, TileShape.Square, Coordinates.From(-4, 3)),
            new(TileColor.Green, TileShape.Diamond, Coordinates.From(-4, 2)),
            new(TileColor.Green, TileShape.FourPointStar, Coordinates.From(-4, 1)),
            new(TileColor.Green, TileShape.EightPointStar, Coordinates.From(-4, 0)),
            new(TileColor.Green, TileShape.Clover, Coordinates.From(-4, -1))
        };
        tilesPlayedOrdered = PlayTilesAndTestPersistence(Player9, tilesToPlay, tilesPlayedOrdered);

        tilesToPlay = new List<TileOnBoard>
        {
            new(TileColor.Blue, TileShape.Circle, Coordinates.From(-5, 4)),
            new(TileColor.Blue, TileShape.Square, Coordinates.From(-5, 3)),
            new(TileColor.Blue, TileShape.Diamond, Coordinates.From(-5, 2))
        };
        _ = PlayTilesAndTestPersistence(Player3, tilesToPlay, tilesPlayedOrdered);
    }

    private static List<TileOnBoardDao> TileOnBoardDaoOrdered(DefaultDbContext _dbContext) => _dbContext.TilesOnBoard.Where(t => t.GameId == GameId).OrderBy(t => t.PositionX).ThenBy(t => t.PositionY).ToList();

    private List<TileOnBoard> Order(List<TileOnBoard> tilesToPlay) => tilesToPlay.OrderBy(t => t.Coordinates.X).ThenBy(t => t.Coordinates.Y).ToList();

    private static void TestEqualityBetweenTilesPlayedAndPersistenceBoard(IReadOnlyCollection<TileOnBoardDao> tilesOnBoard, IReadOnlyCollection<TileOnBoard> tilesPlayed)
    {
        tilesOnBoard.Select(t => t.PositionX).ShouldBe(tilesPlayed.Select(t => t.Coordinates.X));
        tilesOnBoard.Select(t => t.PositionY).ShouldBe(tilesPlayed.Select(t => t.Coordinates.Y));
        tilesOnBoard.Select(t => t.Tile.Shape).ShouldBe(tilesPlayed.Select(t => t.Shape));
        tilesOnBoard.Select(t => t.Tile.Color).ShouldBe(tilesPlayed.Select(t => t.Color));
    }

    private List<TileOnBoard> PlayTilesAndTestPersistence(int playerId, List<TileOnBoard> tilesToPlay, List<TileOnBoard> tilesPlayedOrdered)
    {
        var playReturn = _coreService.TryPlayTiles(playerId, tilesToPlay);
        playReturn.Code.ShouldBe(ReturnCode.Ok);
        tilesPlayedOrdered.AddRange(tilesToPlay);
        tilesPlayedOrdered = Order(tilesPlayedOrdered);
        var tilesOnBoardOrdered = TileOnBoardDaoOrdered(_dbContext);
        TestEqualityBetweenTilesPlayedAndPersistenceBoard(tilesOnBoardOrdered, tilesPlayedOrdered);
        return tilesPlayedOrdered;
    }
}
