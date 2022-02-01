namespace Qwirkle.Test;

public class CreateGameShould
{
    private readonly CoreService _coreService;
    private readonly DefaultDbContext _dbContext;

    private const int User1Id = 71;
    private const int User2Id = 21;
    private const int User3Id = 33;
    private const int User4Id = 14;

    public CreateGameShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        IRepository repository = new Repository(_dbContext);
        var authenticationUseCase = new UserService(new NoRepository(), new FakeAuthentication());
        _coreService = new CoreService(repository, null, null, new Logger<CoreService>(new LoggerFactory()));
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
    public void CreateGoodPlayerWithOrder0()
    {
        var userIds = new HashSet<int> { User3Id };
        var game = _coreService.CreateGame(userIds);

        game.Players.Count.ShouldBe(1);
        game.Players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(1);
        game.Players.Count(p => p.Points == 0).ShouldBe(1);
        game.Players[0].IsTurn.ShouldBe(true);
        game.Players[0].GamePosition.ShouldBe(0);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder01()
    {
        var userIds = new HashSet<int> { User3Id, User4Id };
        var game = _coreService.CreateGame(userIds);

        game.Players.Count.ShouldBe(2);
        game.Players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(2);
        game.Players.Count(p => p.Points == 0).ShouldBe(2);
        game.Players[0].IsTurn.ShouldBe(true);
        game.Players[0].GamePosition.ShouldBe(0);
        game.Players[1].GamePosition.ShouldBe(1);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder012()
    {
        var userIds = new HashSet<int> { User1Id, User3Id, User4Id };
        var game = _coreService.CreateGame(userIds);

        game.Players.Count.ShouldBe(3);
        game.Players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(3);
        game.Players.Count(p => p.Points == 0).ShouldBe(3);
        game.Players[0].IsTurn.ShouldBe(true);
        game.Players[0].GamePosition.ShouldBe(0);
        game.Players[1].GamePosition.ShouldBe(1);
        game.Players[2].GamePosition.ShouldBe(2);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder0123()
    {
        var userIds = new HashSet<int> { User1Id, User2Id, User3Id, User4Id };
        var game = _coreService.CreateGame(userIds);

        game.Players.Count.ShouldBe(4);
        game.Players.Select(p => p.Rack.Tiles.Count == CoreService.TilesNumberPerPlayer).Count().ShouldBe(4);
        game.Players.Count(p => p.Points == 0).ShouldBe(4);
        game.Players[0].IsTurn.ShouldBe(true);
        game.Players[0].GamePosition.ShouldBe(0);
        game.Players[1].GamePosition.ShouldBe(1);
        game.Players[2].GamePosition.ShouldBe(2);
        game.Players[3].GamePosition.ShouldBe(3);
    }
}
