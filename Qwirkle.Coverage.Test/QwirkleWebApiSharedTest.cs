namespace Qwirkle.Coverage.Test;

public class QwirkleWebApiSharedTest
{
    [Fact]
    public void InstantGameModelTest()
    {
        Action action = () => _ = new InstantGameModel();
        action.ShouldNotThrow();

        action = () => _ = new InstantGameModel(true, new[] { "toto" }, -1);
        action.ShouldThrow<ArgumentException>().Message.ShouldBe("gameId Must be > 0 if present"); ;

        action = () => _ = new InstantGameModel(true, new[] { "toto" }, 0);
        action.ShouldThrow<ArgumentException>().Message.ShouldBe("gameId Must be > 0 if present");

        var usersNames = new [] {"player0", "player1"};
        var instantGameModel = new InstantGameModel(true, usersNames);
        instantGameModel.IsAdded.ShouldBe(true);
        instantGameModel.UsersNames.ShouldBe(usersNames);
    }

    [Fact]
    public void ArrangeTileModelTest()
    {
        Action action = () => _ = new ArrangeTileModel();
        action.ShouldNotThrow();

        const byte rackPosition = 0;
        const int gameId = 1;
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var arrangeTileModel = new ArrangeTileModel(gameId, tile, rackPosition);
        var tileOnRack = arrangeTileModel.ToTileOnRack();
        tileOnRack.RackPosition.ShouldBe(rackPosition);
        tileOnRack.Shape.ShouldBe(tile.Shape);
        tileOnRack.Color.ShouldBe(tile.Color);
        arrangeTileModel.GameId.ShouldBe(gameId);
    }

    [Fact]
    public void PlayTileModelTest()
    {
        Action action = () => _ = new PlayTileModel();
        action.ShouldNotThrow();

        const int gameId = 1;
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var playTileModel = new PlayTileModel(gameId, tile, Coordinate.From(0, 0));
        playTileModel.GameId.ShouldBe(gameId);
    }

    [Fact]
    public void RegisterModelTest()
    {
        Action action = () => _ = new RegisterModel();
        action.ShouldNotThrow();

        var registerModel = new RegisterModel { Firstname = "firstname", Lastname = "lastname", UserName = "userName", Email = "email@email" };
        _ = registerModel.ToUser();
    }

    [Fact]
    public void SwapTileModelTest()
    {
        Action action = () => _ = new SwapTileModel();
        action.ShouldNotThrow();

        const int gameId = 1;
        const byte rackPosition = 0;
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var swapTileModel = new SwapTileModel(gameId, tile, rackPosition);
        swapTileModel.GameId.ShouldBe(gameId);
        _ = swapTileModel.ToTileOnRack();
    }

    [Fact]
    public void UserInfoTest()
    {
        const string userName = "John";
        const bool isAuthenticated = true;
        var exposedClaims = new Dictionary<string, string>();
        var userInfo = new UserInfo() { UserName = userName, IsAuthenticated = isAuthenticated, ExposedClaims = exposedClaims };
        userInfo.UserName.ShouldBe(userName);
        userInfo.ExposedClaims.ShouldBe(exposedClaims);
        userInfo.IsAuthenticated.ShouldBe(isAuthenticated);
    }

    [Fact]
    public void OpponentsModelTest()
    {
        var opponentsModel = new OpponentsModel();
        opponentsModel.Opponent1.ShouldBe(string.Empty);
        const string opponent1 = "opponent1";
        const string opponent2 = "opponent2";
        const string opponent3 = "opponent3";
        
        opponentsModel = new OpponentsModel(opponent1, opponent2, opponent3);
        opponentsModel.Opponent1.ShouldBe(opponent1);
        opponentsModel.Opponent2.ShouldBe(opponent2);
        opponentsModel.Opponent3.ShouldBe(opponent3);
    }
}