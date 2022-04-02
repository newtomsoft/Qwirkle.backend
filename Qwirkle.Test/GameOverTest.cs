namespace Qwirkle.Test;

public class GameOverTest
{
    #region private

    private DefaultDbContext _dbContext = null!;
    private Repository _repository = null!;
    private InfoService _infoService = null!;
    private CoreService _coreService = null!;

    private const int TotalTiles = 108;
    private const int GameId = 7;
    private const int User71 = 71;
    private const int User21 = 21;
    private const int Player9 = 9;
    private const int Player3 = 3;

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
        AddGame();
        Add2Players();
        AddTilesOnPlayers();
        AddTilesOnBoard();
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

    private void AddGame()
    {
        _dbContext.Games.Add(new GameDao { Id = GameId });
        _dbContext.SaveChanges();
    }

    private void AddUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User71 });
        _dbContext.Users.Add(new UserDao { Id = User21 });
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
        _dbContext.TilesOnRack.Add(new TileOnRackDao { Id = 1, PlayerId = Player9, TileId = 1 });
        _dbContext.SaveChanges();
    }

    private void AddTilesOnBoard()
    {
        _dbContext.TilesOnBoard.Add(new TileOnBoardDao() { Id = 1, TileId = 2, GameId = GameId, PositionX = 0, PositionY = 0 });
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void GameOverShouldBeTrueAfterPlayerFinishHisRack()
    {
        InitTest();
        var tilesToPlay = new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, Coordinate.From(0, 1)) };
        _infoService.GetGame(GameId).GameOver.ShouldBeFalse();
        _ = _coreService.TryPlayTiles(Player9, tilesToPlay);
        _infoService.GetGame(GameId).GameOver.ShouldBeTrue();
    }
}
