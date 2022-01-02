namespace Qwirkle.Test;

public class CreateGameShould
{
    private readonly CoreUseCase _coreUseCase;
    private readonly DefaultDbContext _dbContext;

    private const int User1Id = 71;
    private const int User2Id = 21;
    private const int User3Id = 3;
    private const int User4Id = 14;
    private const int TilesNumberPerPlayer = 6;

    public CreateGameShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        IRepository repository = new Repository(_dbContext);
        _coreUseCase = new CoreUseCase(repository, null, null);
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
    public void CreateGoodPlayerWithOrder1()
    {
        var userIds = new HashSet<int> { User3Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count(p => p.GamePosition == 1).ShouldBe(1);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.Points > 0).ShouldBe(0);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(1);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder12()
    {
        var userIds = new HashSet<int> { User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count(p => p.GamePosition == 1).ShouldBe(1);
        players.Count(p => p.GamePosition == 2).ShouldBe(1);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.Points > 0).ShouldBe(0);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(2);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder123()
    {
        var userIds = new HashSet<int> { User1Id, User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count(p => p.GamePosition == 1).ShouldBe(1);
        players.Count(p => p.GamePosition == 2).ShouldBe(1);
        players.Count(p => p.GamePosition == 3).ShouldBe(1);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.Points > 0).ShouldBe(0);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(3);

    }

    [Fact]
    public void CreateGoodPlayersWithOrder1234()
    {
        var userIds = new HashSet<int> { User1Id, User2Id, User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count(p => p.GamePosition == 1).ShouldBe(1);
        players.Count(p => p.GamePosition == 2).ShouldBe(1);
        players.Count(p => p.GamePosition == 3).ShouldBe(1);
        players.Count(p => p.GamePosition == 4).ShouldBe(1);
        players.Count(p => p.IsTurn).ShouldBe(1);
        players.Count(p => p.Points > 0).ShouldBe(0);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(4);
    }
}
