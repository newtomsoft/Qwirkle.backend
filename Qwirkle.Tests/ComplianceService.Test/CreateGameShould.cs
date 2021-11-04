using System.Linq;
using Qwirkle.Core.UseCases;

namespace Qwirkle.Core.Tests;

public class CreateGameShould
{
    private readonly CoreUseCase _commonUseCase;
    private readonly DefaultDbContext _dbContext;

    private const int User1 = 71;
    private const int User2 = 21;
    private const int User3 = 3;
    private const int User4 = 14;
    private const int TilesNumberPerPlayer = 6;

    public CreateGameShould()
    {
        _dbContext = ConnectionFactory.CreateContextForInMemory();
        IRepository repository = new Repository(_dbContext);
        _commonUseCase = new CoreUseCase(repository);
        AddUsers();
    }

    private void AddUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User1 });
        _dbContext.Users.Add(new UserDao { Id = User2 });
        _dbContext.Users.Add(new UserDao { Id = User3 });
        _dbContext.Users.Add(new UserDao { Id = User4 });
        _dbContext.SaveChanges();
    }

    [Fact]
    public void CreateGoodPlayersWithOrder1234()
    {
        var userIds = new List<int> { User1, User2, User3, User4 };
        var players = _commonUseCase.CreateGame(userIds);

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
        var userIds = new List<int> { User1, User3, User4 };
        var players = _commonUseCase.CreateGame(userIds);

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
        var userIds = new List<int> { User3, User4 };
        var players = _commonUseCase.CreateGame(userIds);

        Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 2);
        Assert.Equal(1, players.Count(p => p.IsTurn));
        Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
        Assert.Equal(2, players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer).Count());
    }

    [Fact]
    public void CreateGoodPlayerWithOrder1()
    {
        var userIds = new List<int> { User3 };
        var players = _commonUseCase.CreateGame(userIds);
        Assert.Contains(players.Select(p => p.GamePosition), value => value == 1);
        Assert.Equal(1, players.Count(p => p.IsTurn));
        Assert.DoesNotContain(players.Select(p => p.Points), points => points > 0);
        Assert.Single(players.Select(p => p.Rack.Tiles.Count == TilesNumberPerPlayer));
    }
}
