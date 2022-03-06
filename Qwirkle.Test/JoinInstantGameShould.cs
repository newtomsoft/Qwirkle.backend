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
    public void ReturnUserNameWhen1UserJoinGame(int playersNumber)
    {
        InitTest();
        const string userName = "UserName";
        var usersNames = _instantGameService.JoinInstantGame(userName, playersNumber);
        usersNames.Count.ShouldBe(1);
        usersNames.First().ShouldBe(userName);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUserNameWhen1UserJoinGameManyTime(int playersNumber)
    {
        InitTest();
        const string userName = "UserName";
        var usersNames = new HashSet<string>();
        const int testRepeat = 100;
        for (var iTest = 0; iTest < testRepeat; iTest++)
        {
            var manyTime = new Random().Next(50, 100); // no matter how many
            for (var i = 0; i < manyTime; i++) usersNames = _instantGameService.JoinInstantGame(userName, playersNumber);
            usersNames.Count.ShouldBe(1);
            usersNames.First().ShouldBe(userName);
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUsersNamesWhen2UsersJoinGame(int playersNumber)
    {
        InitTest();
        const string user0 = "userName0";
        const string user1 = "userName1";
        var expectedUsersNames = new HashSet<string> { user0, user1 };
        var resultUsersIds = _instantGameService.JoinInstantGame(user0, playersNumber);
        resultUsersIds.Count.ShouldBe(1);
        resultUsersIds = _instantGameService.JoinInstantGame(user1, playersNumber);
        resultUsersIds.Count.ShouldBe(2);
        resultUsersIds.OrderBy(id => id).ShouldBe(expectedUsersNames.OrderBy(id => id));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ReturnUsersIdsWhen3UsersJoinGame(int playersNumber)
    {
        InitTest();
        const string user0 = "userName0";
        const string user1 = "userName1";
        const string user2 = "userName2";
        var expectedUsersNames = new HashSet<string> { user0, user1, user2 };
        _instantGameService.JoinInstantGame(user0, playersNumber);
        var result0UsersIds = _instantGameService.JoinInstantGame(user1, playersNumber);
        var result1UsersIds = _instantGameService.JoinInstantGame(user2, playersNumber);
        result1UsersIds.Count.ShouldBe(playersNumber < 3 ? 1 : 3);
        var resultUsersIds = result0UsersIds.Union(result1UsersIds);
        resultUsersIds.OrderBy(id => id).ShouldBe(expectedUsersNames.OrderBy(id => id));
    }

    [Theory]
    [InlineData(4)]
    public void ReturnUsersIdsWhen4UsersJoinGame(int playersNumber)
    {
        InitTest();
        const string user0 = "userName0";
        const string user1 = "userName1";
        const string user2 = "userName2";
        const string user3 = "userName3";
        var expectedUsersNames = new HashSet<string> { user0, user1, user2, user3 };
        _instantGameService.JoinInstantGame(user0, playersNumber);
        _instantGameService.JoinInstantGame(user1, playersNumber);
        _instantGameService.JoinInstantGame(user2, playersNumber);
        var resultUsersIds = _instantGameService.JoinInstantGame(user3, playersNumber);
        resultUsersIds.Count.ShouldBe(playersNumber);
        resultUsersIds.OrderBy(id => id).ShouldBe(expectedUsersNames.OrderBy(id => id));
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
            var resultUsersNames = new HashSet<string>();
            for (var id = 1; id <= userNumber; id++)
            {
                var userName = "user" + id;
                resultUsersNames = _instantGameService.JoinInstantGame(userName, playersNumberInGame);
                if (resultUsersNames.Count == playersNumberInGame) gamesNumberToCreate++;
            }
            resultUsersNames.Count.ShouldBe(userNumber % playersNumberInGame == 0 ? playersNumberInGame : userNumber % playersNumberInGame);
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
        var resultUsersIds = new HashSet<string>[userNumber + 1];
        var badGamesNumber = 0;
        Parallel.ForEach(usersIds, id =>
        {
            var userName = "user" + id;
            resultUsersIds[id] = _instantGameService.JoinInstantGame(userName, playersNumberInGame);
            if (resultUsersIds[id].Count > playersNumberInGame) badGamesNumber++;
        });
        resultUsersIds[0] = new HashSet<string>();
        resultUsersIds.Count(e => e.Count == playersNumberInGame).ShouldBe(userNumber / playersNumberInGame);
        badGamesNumber.ShouldBe(0);
    }
}
