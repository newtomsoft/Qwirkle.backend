namespace Qwirkle.WebApi.Client.Blazor.Tests.ServiceTests;

public class PlayersDetailTests
{
    private const int Position0 = 0;
    private const int Position1 = 1;
    private const int Position2 = 2;
    private const int Position3 = 3;
    private const string PlayerPseudo = "testPlayer";
    private const string Opponent0Pseudo = "opponent0";
    private const string Opponent1Pseudo = "opponent1";
    private const string Opponent2Pseudo = "opponent2";

    [Fact(DisplayName = "Initialize should order player-opponent0-opponent1-opponent2")]
    public void InitializeTest0()
    {
        var player = new Player(1, 1, 1, PlayerPseudo, Position0, 0, 0, Rack.Empty, false, false);
        var opponent0 = new Player(2, 2, 1, Opponent0Pseudo, Position1, 0, 0, Rack.Empty, true, false);
        var opponent1 = new Player(3, 3, 1, Opponent1Pseudo, Position2, 0, 0, Rack.Empty, false, false);
        var opponent2 = new Player(4, 4, 1, Opponent2Pseudo, Position3, 0, 0, Rack.Empty, false, false);
        IEnumerable<PlayerDetail> inputPlayersDetails = new[] { new PlayerDetail(opponent0), new PlayerDetail(opponent1), new PlayerDetail(opponent2), new PlayerDetail(player) };
        IPlayersDetail playersDetail = new PlayersDetail();
       
        playersDetail.Initialize(inputPlayersDetails, PlayerPseudo);
        playersDetail.All.Length.ShouldBe(4);
        playersDetail.All[0].Pseudo.ShouldBe(PlayerPseudo);
        playersDetail.All[1].Pseudo.ShouldBe(Opponent0Pseudo);
        playersDetail.All[2].Pseudo.ShouldBe(Opponent1Pseudo);
        playersDetail.All[3].Pseudo.ShouldBe(Opponent2Pseudo);
    }

    [Fact(DisplayName = "Initialize should order player-opponent1-opponent2-opponent0")]
    public void InitializeTest1()
    {
        var opponent0 = new Player(2, 2, 1, Opponent0Pseudo, Position0, 0, 0, Rack.Empty, true, false);
        var player = new Player(1, 1, 1, PlayerPseudo, Position1, 0, 0, Rack.Empty, false, false);
        var opponent1 = new Player(3, 3, 1, Opponent1Pseudo, Position2, 0, 0, Rack.Empty, false, false);
        var opponent2 = new Player(4, 4, 1, Opponent2Pseudo, Position3, 0, 0, Rack.Empty, false, false);
        IEnumerable<PlayerDetail> inputPlayersDetails = new[] {new PlayerDetail(opponent0), new PlayerDetail(opponent1), new PlayerDetail(opponent2), new PlayerDetail(player)};
        IPlayersDetail playersDetail = new PlayersDetail();
        
        playersDetail.Initialize(inputPlayersDetails, PlayerPseudo);
        playersDetail.All.Length.ShouldBe(4);
        playersDetail.All[0].Pseudo.ShouldBe(PlayerPseudo);
        playersDetail.All[1].Pseudo.ShouldBe(Opponent1Pseudo);
        playersDetail.All[2].Pseudo.ShouldBe(Opponent2Pseudo);
        playersDetail.All[3].Pseudo.ShouldBe(Opponent0Pseudo);
    }

    [Fact(DisplayName = "Initialize should order player-opponent2-opponent0-opponent1")]
    public void InitializeTest2()
    {
        var opponent0 = new Player(2, 2, 1, Opponent0Pseudo, Position0, 0, 0, Rack.Empty, true, false);
        var opponent1 = new Player(3, 3, 1, Opponent1Pseudo, Position1, 0, 0, Rack.Empty, false, false);
        var player = new Player(1, 1, 1, PlayerPseudo, Position2, 0, 0, Rack.Empty, false, false);
        var opponent2 = new Player(4, 4, 1, Opponent2Pseudo, Position3, 0, 0, Rack.Empty, false, false);
        IEnumerable<PlayerDetail> inputPlayersDetails = new[] { new PlayerDetail(opponent0), new PlayerDetail(opponent1), new PlayerDetail(opponent2), new PlayerDetail(player) };
        IPlayersDetail playersDetail = new PlayersDetail();
        
        playersDetail.Initialize(inputPlayersDetails, PlayerPseudo);
        playersDetail.All.Length.ShouldBe(4);
        playersDetail.All[0].Pseudo.ShouldBe(PlayerPseudo);
        playersDetail.All[1].Pseudo.ShouldBe(Opponent2Pseudo);
        playersDetail.All[2].Pseudo.ShouldBe(Opponent0Pseudo);
        playersDetail.All[3].Pseudo.ShouldBe(Opponent1Pseudo);
    }

    [Fact(DisplayName = "Initialize should order player-opponent0-opponent1-opponent2")]
    public void InitializeTest3()
    {
        var opponent0 = new Player(2, 2, 1, Opponent0Pseudo, Position0, 0, 0, Rack.Empty, true, false);
        var opponent1 = new Player(3, 3, 1, Opponent1Pseudo, Position1, 0, 0, Rack.Empty, false, false);
        var opponent2 = new Player(4, 4, 1, Opponent2Pseudo, Position2, 0, 0, Rack.Empty, false, false);
        var player = new Player(1, 1, 1, PlayerPseudo, Position3, 0, 0, Rack.Empty, false, false);
        IEnumerable<PlayerDetail> inputPlayersDetails = new[] { new PlayerDetail(opponent0), new PlayerDetail(opponent1), new PlayerDetail(opponent2), new PlayerDetail(player) };
        IPlayersDetail playersDetail = new PlayersDetail();
        
        playersDetail.Initialize(inputPlayersDetails, PlayerPseudo);
        playersDetail.All.Length.ShouldBe(4);
        playersDetail.All[0].Pseudo.ShouldBe(PlayerPseudo);
        playersDetail.All[1].Pseudo.ShouldBe(Opponent0Pseudo);
        playersDetail.All[2].Pseudo.ShouldBe(Opponent1Pseudo);
        playersDetail.All[3].Pseudo.ShouldBe(Opponent2Pseudo);
    }

    [Fact]
    public void OnPlayerPointsChangedTest()
    {
        const int playerInitialPoints = 3;
        const int opponent2InitialPoints = 1;
        var player = new Player(1, 1, 1, PlayerPseudo, Position0, playerInitialPoints, 0, Rack.Empty, false, false);
        var opponent0 = new Player(2, 2, 1, Opponent0Pseudo, Position1, 0, 0, Rack.Empty, true, false);
        var opponent1 = new Player(3, 3, 1, Opponent1Pseudo, Position2, 0, 0, Rack.Empty, false, false);
        var opponent2 = new Player(4, 4, 1, Opponent2Pseudo, Position3, opponent2InitialPoints, 0, Rack.Empty, false, false);
        IEnumerable<PlayerDetail> inputPlayersDetails = new[] { new PlayerDetail(opponent0), new PlayerDetail(opponent1), new PlayerDetail(opponent2), new PlayerDetail(player) };
        IPlayersDetail playersDetail = new PlayersDetail();
        playersDetail.Initialize(inputPlayersDetails, PlayerPseudo);

        const int pointsToAdd = 5;
        playersDetail.OnPlayerPointsChanged(new object(), new PlayerPointsChangedEventArgs(PlayerPseudo, pointsToAdd));
        playersDetail.All[0].Points.ShouldBe(playerInitialPoints + pointsToAdd);

        const int pointsToAdd2 = 5;
        playersDetail.OnPlayerPointsChanged(new object(), new PlayerPointsChangedEventArgs(Opponent2Pseudo, pointsToAdd2));
        playersDetail.All[3].Points.ShouldBe(opponent2InitialPoints + pointsToAdd2);
    }

    [Fact]
    public void OnPlayerTurnChangedTest()
    {
        const int playerInitialPoints = 3;
        const int opponent2InitialPoints = 1;
        var player = new Player(1, 1, 1, PlayerPseudo, Position0, playerInitialPoints, 0, Rack.Empty, false, false);
        var opponent0 = new Player(2, 2, 1, Opponent0Pseudo, Position1, 0, 0, Rack.Empty, true, false);
        var opponent1 = new Player(3, 3, 1, Opponent1Pseudo, Position2, 0, 0, Rack.Empty, false, false);
        var opponent2 = new Player(4, 4, 1, Opponent2Pseudo, Position3, opponent2InitialPoints, 0, Rack.Empty, false, false);
        IEnumerable<PlayerDetail> inputPlayersDetails = new[] { new PlayerDetail(opponent0), new PlayerDetail(opponent1), new PlayerDetail(opponent2), new PlayerDetail(player) };
        IPlayersDetail playersDetail = new PlayersDetail();
        playersDetail.Initialize(inputPlayersDetails, PlayerPseudo);

        playersDetail.OnPlayerTurnChanged(new object(), new PlayerTurnChangedEventArgs(PlayerPseudo));
        playersDetail.All.Single(p=> p.IsTurn).Pseudo.ShouldBe(PlayerPseudo);

        playersDetail.OnPlayerTurnChanged(new object(), new PlayerTurnChangedEventArgs(Opponent0Pseudo));
        playersDetail.All.Single(p => p.IsTurn).Pseudo.ShouldBe(Opponent0Pseudo);
        
        playersDetail.OnPlayerTurnChanged(new object(), new PlayerTurnChangedEventArgs(Opponent1Pseudo));
        playersDetail.All.Single(p => p.IsTurn).Pseudo.ShouldBe(Opponent1Pseudo);
        
        playersDetail.OnPlayerTurnChanged(new object(), new PlayerTurnChangedEventArgs(Opponent2Pseudo));
        playersDetail.All.Single(p => p.IsTurn).Pseudo.ShouldBe(Opponent2Pseudo);
    }
}
