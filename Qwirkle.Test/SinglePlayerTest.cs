namespace Qwirkle.Test;

public class SinglePlayerTest
{
    #region private

    private DefaultDbContext _dbContext = null!;
    private Repository _repository = null!;
    private InfoService _infoService = null!;
    private CoreService _coreService = null!;

    private const int User0Id = 71;

    private void InitTest()
    {
        InitDbContext();
        InitData();
        _repository = new Repository(_dbContext);
        _infoService = new InfoService(_repository, null, new Logger<InfoService>(new LoggerFactory()));
        _coreService = new CoreService(_repository, new NoNotification(), _infoService, null, new Logger<CoreService>(new LoggerFactory()));
    }

    private void InitDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new DefaultDbContext(contextOptions);
    }

    private void InitData()
    {
        AddAllTiles();
        AddUsers();
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

    private void AddUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User0Id });
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact(DisplayName = "Player turn should remain the same after swap")]
    public void SwapTest()
    {
        InitTest();
        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id });
        var players = _infoService.GetGame(gameId).Players;
        var player = players.Single(p => p.IsTurn);

        var tileToSwap = player.Rack.Tiles[0];
        var tilesToSwap = new List<Tile> { tileToSwap };
        _ = _coreService.TrySwapTiles(player.Id, tilesToSwap);

        var game = _repository.GetGame(player.GameId);
        game.Players[0].IsTurn.ShouldBeTrue();
    }

    [Fact(DisplayName = "Player turn should remain the same after play")]
    public void PlayTest()
    {
        InitTest();
        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id });
        var players = _infoService.GetGame(gameId).Players;
        var player = players.Single(p => p.IsTurn);

        var tilesToPlay = new List<TileOnBoard>
        { new(TileColor.Green, TileShape.Circle, Coordinate.From(-4, 4)),
            new(TileColor.Green, TileShape.Square, Coordinate.From(-4, 3)),
            new(TileColor.Green, TileShape.Diamond, Coordinate.From(-4, 2)),
            new(TileColor.Green, TileShape.FourPointStar, Coordinate.From(-4, 1)),
            new(TileColor.Green, TileShape.EightPointStar, Coordinate.From(-4, 0)),
            new(TileColor.Green, TileShape.Clover, Coordinate.From(-4, -1))
        };
        _ = _coreService.TryPlayTiles(player.Id, tilesToPlay);

        players = _infoService.GetGame(gameId).Players;
        players[0].IsTurn.ShouldBeTrue();
    }
}
