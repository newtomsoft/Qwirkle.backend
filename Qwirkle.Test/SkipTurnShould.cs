namespace Qwirkle.Test;

public class SkipTurnShould
{
    #region private

    private DefaultDbContext _dbContext = null!;
    private Repository _repository = null!;
    private InfoService _infoService = null!;
    private CoreService _coreService = null!;

    private const int User0Id = 71;
    private const int User1Id = 72;

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
        _dbContext.Users.Add(new UserDao { Id = User1Id });
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void ReturnNotPlayerTurnWhenItsNotTurnPlayer()
    {
        InitTest();
        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var players = _infoService.GetGame(gameId).Players;
        var player = players.First(p => p.IsTurn is false);
        var swapReturn = _coreService.TrySkipTurn(player.Id);
        swapReturn.Code.ShouldBe(ReturnCode.NotPlayerTurn);
    }

    [Fact]
    public void ReturnOkWhenItsPlayerTurn()
    {
        for (var i = 0; i < CoreService.TilesNumberPerPlayer; i++)
        {
            InitTest();
            var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
            var players = _infoService.GetGame(gameId).Players;
            var player = players.Single(p => p.IsTurn);

            var skipTurnReturn = _coreService.TrySkipTurn(player.Id);
            skipTurnReturn.Code.ShouldBe(ReturnCode.Ok);
        }
    }
}
