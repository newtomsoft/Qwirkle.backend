namespace Qwirkle.Test;

public class JoinInstantGameShould
{
    #region private
    private InstantGameService _instantGameService = null!;

    private void InitTest()
    {
        _instantGameService = new InstantGameService(null);
    }

    #endregion

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUserIdWhen1UserJoinGame(int playersNumber)
    {
        InitTest();
        const int userId = 10;
        var usersIds = _instantGameService.JoinInstantGame(userId, playersNumber);
        usersIds.Count.ShouldBe(1);
        usersIds.First().ShouldBe(userId);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUserIdWhen1UserJoinGameManyTime(int playersNumber)
    {
        InitTest();
        const int userId = 10;
        var usersIds = new HashSet<int>();
        const int testRepeat = 100;
        for (var iTest = 0; iTest < testRepeat; iTest++)
        {
            var manyTime = new Random().Next(50, 100); // no matter how many
            for (var i = 0; i < manyTime; i++) usersIds = _instantGameService.JoinInstantGame(userId, playersNumber);
            usersIds.Count.ShouldBe(1);
            usersIds.First().ShouldBe(userId);
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUsersIdsWhen2UsersJoinGame(int playersNumber)
    {
        InitTest();
        const int user0Id = 10;
        const int user1Id = 22;
        var expectedUsersIds = new HashSet<int> { user0Id, user1Id };
        var resultUsersIds = _instantGameService.JoinInstantGame(user0Id, playersNumber);
        resultUsersIds.Count.ShouldBe(1);
        resultUsersIds = _instantGameService.JoinInstantGame(user1Id, playersNumber);
        resultUsersIds.Count.ShouldBe(2);
        resultUsersIds.OrderBy(id => id).ShouldBe(expectedUsersIds.OrderBy(id => id));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUsersIdsWhen3UsersJoinGame(int playersNumber)
    {
        InitTest();
        const int user0Id = 10;
        const int user1Id = 22;
        const int user2Id = 69;
        var expectedUsersIds = new HashSet<int> { user0Id, user1Id, user2Id };
        _instantGameService.JoinInstantGame(user0Id, playersNumber);
        var result0UsersIds = _instantGameService.JoinInstantGame(user1Id, playersNumber);
        var result1UsersIds = _instantGameService.JoinInstantGame(user2Id, playersNumber);
        result1UsersIds.Count.ShouldBe(playersNumber < 3 ? 1 : 3);
        var resultUsersIds = result0UsersIds.Union(result1UsersIds);
        resultUsersIds.OrderBy(id => id).ShouldBe(expectedUsersIds.OrderBy(id => id));

    }

    [Theory]
    [InlineData(4)]
    public void ReturnUsersIdsWhen4UsersJoinGame(int playersNumber)
    {
        InitTest();
        const int user0Id = 10;
        const int user1Id = 22;
        const int user2Id = 69;
        const int user3Id = 42;
        var expectedUsersIds = new HashSet<int> { user0Id, user1Id, user2Id, user3Id };
        _instantGameService.JoinInstantGame(user0Id, playersNumber);
        _instantGameService.JoinInstantGame(user1Id, playersNumber);
        _instantGameService.JoinInstantGame(user2Id, playersNumber);
        var resultUsersIds = _instantGameService.JoinInstantGame(user3Id, playersNumber);
        resultUsersIds.Count.ShouldBe(playersNumber);
        resultUsersIds.OrderBy(id => id).ShouldBe(expectedUsersIds.OrderBy(id => id));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUsersIdsWhenLotOffUsersJoinGame(int playersNumberInGame)
    {
        const int maxUser = 100;
        for (var userNumber = 1; userNumber <= maxUser; userNumber++)
        {
            InitTest();
            var gamesNumberToCreate = 0;
            var resultUsersIds = new HashSet<int>();
            for (var id = 1; id <= userNumber; id++)
            {
                resultUsersIds = _instantGameService.JoinInstantGame(id, playersNumberInGame);
                if (resultUsersIds.Count == playersNumberInGame) gamesNumberToCreate++;
            }
            resultUsersIds.Count.ShouldBe(userNumber % playersNumberInGame == 0 ? playersNumberInGame : userNumber % playersNumberInGame);
            gamesNumberToCreate.ShouldBe(userNumber / playersNumberInGame);
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUsersIdsWhenLotOffUsersJoinGameInParallel(int playersNumberInGame)
    {
        InitTest();
        const int userNumber = 234567;
        var usersIds = Enumerable.Range(1, userNumber).ToArray();
        var resultUsersIds = new HashSet<int>[userNumber + 1];
        var badGamesNumber = 0;
        Parallel.ForEach(usersIds, id =>
        {
            resultUsersIds[id] = _instantGameService.JoinInstantGame(id, playersNumberInGame);
            if (resultUsersIds[id].Count > playersNumberInGame) badGamesNumber++;
        });
        resultUsersIds[0] = new HashSet<int>();
        resultUsersIds.Count(e => e.Count == playersNumberInGame).ShouldBe(userNumber / playersNumberInGame);
        badGamesNumber.ShouldBe(0);
    }
}
