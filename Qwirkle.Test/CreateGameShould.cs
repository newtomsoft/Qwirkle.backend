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
    public void CreateGoodPlayerWithOrder0()
    {
        var userIds = new HashSet<int> { User3Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count.ShouldBe(1);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(1);
        players.Count(p => p.Points == 0).ShouldBe(1);
        players[0].IsTurn.ShouldBe(true);
        players[0].GamePosition.ShouldBe(0);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder01()
    {
        var userIds = new HashSet<int> { User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count.ShouldBe(2);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(2);
        players.Count(p => p.Points == 0).ShouldBe(2);
        players[0].IsTurn.ShouldBe(true);
        players[0].GamePosition.ShouldBe(0);
        players[1].GamePosition.ShouldBe(1);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder012()
    {
        var userIds = new HashSet<int> { User1Id, User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count.ShouldBe(3);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(3);
        players.Count(p => p.Points == 0).ShouldBe(3);
        players[0].IsTurn.ShouldBe(true);
        players[0].GamePosition.ShouldBe(0);
        players[1].GamePosition.ShouldBe(1);
        players[2].GamePosition.ShouldBe(2);
    }

    [Fact]
    public void CreateGoodPlayersWithOrder0123()
    {
        var userIds = new HashSet<int> { User1Id, User2Id, User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        players.Count.ShouldBe(4);
        players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count().ShouldBe(4);
        players.Count(p => p.Points == 0).ShouldBe(4);
        players[0].IsTurn.ShouldBe(true);
        players[0].GamePosition.ShouldBe(0);
        players[1].GamePosition.ShouldBe(1);
        players[2].GamePosition.ShouldBe(2);
        players[3].GamePosition.ShouldBe(3);
    }
}
