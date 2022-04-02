namespace Qwirkle.Test;

public class CreateGameWithOnlyBots
{
    private readonly CoreService _coreService;
    private readonly DefaultDbContext _dbContext;
    private readonly InfoService _infoService;
    private const int User1Id = 71;
    private const int User2Id = 21;
    private const int User3Id = 33;
    private const int User4Id = 14;

    public CreateGameWithOnlyBots()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        IRepository repository = new Repository(_dbContext);
        _infoService = new InfoService(repository, null, new Logger<InfoService>(new LoggerFactory()));
        _coreService = new CoreService(repository, null, null, null, new Logger<CoreService>(new LoggerFactory()));
        Add4DefaultTestUsers();
    }

    private void Add4DefaultTestUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User1Id });
        _dbContext.Users.Add(new UserDao { Id = User2Id });
        _dbContext.Users.Add(new UserDao { Id = User3Id });
        _dbContext.Users.Add(new UserDao { Id = User4Id });
        _dbContext.SaveChanges();
    }

    [Fact]
    public void CreateGoodPlayerWithPosition0()
    {
        var userIds = new HashSet<int> { User3Id };
        var gameId = _coreService.CreateGame(userIds);
        var players = _infoService.GetGame(gameId).Players;
        players.Count.ShouldBe(1);
        players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(1);
        players.Count(p => p.Points == 0).ShouldBe(1);
        players.First().IsTurn.ShouldBe(true);
        players.First().GamePosition.ShouldBe(0);
    }

    [Fact]
    public void CreateGoodPlayersWithPositions01()
    {
        var userIds = new HashSet<int> { User3Id, User4Id };
        var gameId = _coreService.CreateGame(userIds);
        var players = _infoService.GetGame(gameId).Players;
        players.Count.ShouldBe(2);
        players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(2);
        players.Count(p => p.Points == 0).ShouldBe(2);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.GamePosition == 0).ShouldBe(1);
        players.Count(p => p.GamePosition == 1).ShouldBe(1);
    }

    [Fact]
    public void CreateGoodPlayersWithPositions012()
    {
        var userIds = new HashSet<int> { User1Id, User3Id, User4Id };
        var gameId = _coreService.CreateGame(userIds);
        var players = _infoService.GetGame(gameId).Players;
        players.Count.ShouldBe(3);
        players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(3);
        players.Count(p => p.Points == 0).ShouldBe(3);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.GamePosition == 0).ShouldBe(1);
        players.Count(p => p.GamePosition == 1).ShouldBe(1);
        players.Count(p => p.GamePosition == 2).ShouldBe(1);
    }

    [Fact]
    public void CreateGoodPlayersWithPositions0123()
    {
        var userIds = new HashSet<int> { User1Id, User2Id, User3Id, User4Id };
        var gameId = _coreService.CreateGame(userIds);
        var players = _infoService.GetGame(gameId).Players;
        players.Count.ShouldBe(4);
        players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(4);
        players.Count(p => p.Points == 0).ShouldBe(4);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.GamePosition == 0).ShouldBe(1);
        players.Count(p => p.GamePosition == 1).ShouldBe(1);
        players.Count(p => p.GamePosition == 2).ShouldBe(1);
        players.Count(p => p.GamePosition == 3).ShouldBe(1);
    }
}
