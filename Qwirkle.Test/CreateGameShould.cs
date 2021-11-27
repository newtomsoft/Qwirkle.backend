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
        _coreUseCase = new CoreUseCase(repository, null);
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
    public void CreateGoodPlayersWithOrder1234()
    {
        var userIds = new List<int> { User1Id, User2Id, User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 4);
        Assert.Equal(1, players.Count(p => p.IsTurn));
        Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
        Assert.Equal(4, players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count());

    }

    [Fact]
    public void CreateGoodPlayersWithOrder123()
    {
        var userIds = new List<int> { User1Id, User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 3);
        Assert.Equal(1, players.Count(p => p.IsTurn));
        Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
        Assert.Equal(3, players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count());
    }

    [Fact]
    public void CreateGoodPlayersWithOrder12()
    {
        var userIds = new List<int> { User3Id, User4Id };
        var players = _coreUseCase.CreateGame(userIds);

        Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
        Assert.Equal(1, players.Count(p => p.IsTurn));
        Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
        Assert.Equal(2, players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count());
    }

    [Fact]
    public void CreateGoodPlayerWithOrder1()
    {
        var userIds = new List<int> { User3Id };
        var players = _coreUseCase.CreateGame(userIds);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
        Assert.Equal(1, players.Count(p => p.IsTurn));
        Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
        Assert.Single(players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer));
    }
}
