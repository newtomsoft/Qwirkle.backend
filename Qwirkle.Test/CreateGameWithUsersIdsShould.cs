namespace Qwirkle.Test;

public class CreateGameWithUsersIdsShould
{
    #region privates
    private readonly CoreService _coreService;
    private readonly DefaultDbContext _dbContext;

    private const int Bot1Id = 10;
    private const int Bot2Id = 11;
    private const int Bot3Id = 12;
    private const int Bot4Id = 13;
    private const int Human1Id = 100;
    private const int Human2Id = 101;
    private const int Human3Id = 102;
    private const int Human4Id = 103;

    public CreateGameWithUsersIdsShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        IRepository repository = new Repository(_dbContext);
        var infoService = new InfoService(repository, null, new Logger<InfoService>(new LoggerFactory()));
        var mockAuthentication = CreateMockAuthentication();
        var userService = new UserService(repository, mockAuthentication);
        var notificationService = new NoNotification();
        _coreService = new CoreService(repository, notificationService, infoService, userService, new Logger<CoreService>(new LoggerFactory()));
        AddUsers();
    }

    private static IAuthentication CreateMockAuthentication()
    {
        var fakeAuthentication = Mock.Of<IAuthentication>();
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Bot1Id)).Returns(true);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Bot2Id)).Returns(true);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Bot3Id)).Returns(true);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Bot4Id)).Returns(true);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Human1Id)).Returns(false);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Human2Id)).Returns(false);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Human3Id)).Returns(false);
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(Human4Id)).Returns(false);
        return fakeAuthentication;
    }

    private void AddUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = Bot1Id });
        _dbContext.Users.Add(new UserDao { Id = Bot2Id });
        _dbContext.Users.Add(new UserDao { Id = Bot3Id });
        _dbContext.Users.Add(new UserDao { Id = Bot4Id });
        _dbContext.Users.Add(new UserDao { Id = Human1Id });
        _dbContext.Users.Add(new UserDao { Id = Human2Id });
        _dbContext.Users.Add(new UserDao { Id = Human3Id });
        _dbContext.Users.Add(new UserDao { Id = Human4Id });
        _dbContext.SaveChanges();
    }
    #endregion

    public static TheoryData<HashSet<int>> UsersOnGames => new()
    {
        new HashSet<int> { Bot1Id },
        new HashSet<int> { Bot1Id, Bot2Id },
        new HashSet<int> { Bot1Id, Bot2Id, Bot3Id },
        new HashSet<int> { Bot1Id, Bot2Id, Bot3Id, Bot4Id },
    };
    [Theory(DisplayName = "test with n players all bots")]
    [MemberData(nameof(UsersOnGames))]
    public async Task ReturnFinishGameWhenPlayersAreBots(HashSet<int> usersIds)
    {
        var usersNumber = usersIds.Count;
        var gameId = await _coreService.CreateGameAsync(usersIds);

        gameId.ShouldBe(1);
        var gameDao = _dbContext.Games.First(game => game.Id == 1);
        gameDao.GameOver.ShouldBe(true);
        gameDao.TilesOnBoard.Count.ShouldBeGreaterThanOrEqualTo(CoreService.TotalTilesNumber - 6 * (usersNumber - 1));
        gameDao.TilesOnBag.Count.ShouldBe(0);
        gameDao.Players.All(p => p.Points > CoreService.TotalTilesNumber / usersNumber).ShouldBeTrue();

        var tilesOnPlayers = new Dictionary<int, List<TileOnPlayerDao>>();
        for (var i = 0; i < usersNumber; i++)
            tilesOnPlayers.Add(i, _dbContext.TilesOnPlayer.Where(t => t.PlayerId == gameDao.Players[i].Id).ToList());

        tilesOnPlayers.Count(t => t.Value.Count == 0).ShouldBe(1);
    }
}
