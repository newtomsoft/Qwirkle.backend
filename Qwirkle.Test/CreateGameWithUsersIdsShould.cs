using Moq;

namespace Qwirkle.Test;

public class CreateGameWithUsersIdsShould
{
    private readonly CoreService _coreService;
    private readonly DefaultDbContext _dbContext;

    private const int User1Id = 71;
    private const int User2Id = 21;
    private const int User3Id = 33;
    private const int User4Id = 14;

    public CreateGameWithUsersIdsShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        IRepository repository = new Repository(_dbContext);
        var infoService = new InfoService(repository, null, new Logger<InfoService>(new LoggerFactory()));
        var fakeAuthentication = Mock.Of<IAuthentication>();
        Mock.Get(fakeAuthentication).Setup(m => m.IsBot(User1Id)).Returns(true);
        var userService = new UserService(repository, fakeAuthentication);
        _coreService = new CoreService(repository, null, infoService, userService, new Logger<CoreService>(new LoggerFactory()));
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
        var userIds = new HashSet<int> { User1Id };
        var gameId = _coreService.CreateGameWithUsersIds(userIds);
        
    }


}
