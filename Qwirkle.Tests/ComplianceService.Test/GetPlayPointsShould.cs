namespace Qwirkle.Core.Tests;

public class GetPlayPointsShould
{
    private const int QwirklePoints = 6;
    private CoreUseCase UseCase { get; } = new CoreUseCase(null, null);

    [Fact]
    public void ReturnNumberOfTilesWhenGameIsEmptyAndTilesMakeRow()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard>(), new List<Player>(), false);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(1, 5)) }, null).Points.ShouldBe(1);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(13, 69)), new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(12, 69)) }, null).Points.ShouldBe(2);
    }

    [Fact]
    public void Return0WhenGameIsEmptyAndTilesNotInRow()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard>(), new List<Player>(), false);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(1, 5)), new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(2, 4)) }, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(8, 12)), new TileOnBoard(TileColor.Yellow, TileShape.Circle, new CoordinatesInGame(9, 12)), new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(6, 12)) }, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreInTheSamePlace()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard>(), new List<Player>(), false);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileShape.FourPointStar, new CoordinatesInGame(6, -3)) }, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileShape.FourPointStar, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileShape.Diamond, new CoordinatesInGame(5, -3)) }, null).Points.ShouldBe(0);

        UseCase.Game = new Game(1, new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -3)), }, new List<Player>(), false);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileShape.FourPointStar, new CoordinatesInGame(6, -3)) }, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileShape.FourPointStar, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileShape.Diamond, new CoordinatesInGame(5, -3)) }, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return2When1GoodTileIsAround1TileOnGame()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>(), false);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(8, -3)) }, null).Points.ShouldBe(2);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(6, -3)) }, null).Points.ShouldBe(2);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -4)) }, null).Points.ShouldBe(2);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -2)) }, null).Points.ShouldBe(2);
    }

    [Fact]
    public void Return0WhenTileIsOverOtherTileOnGame()
    {
        var coordinatesNotFree1 = new CoordinatesInGame(0, 0);
        var coordinatesNotFree2 = new CoordinatesInGame(14, 28);
        var coordinatesNotFree3 = new CoordinatesInGame(-7, 3);
        var coordinatesNotFree4 = new CoordinatesInGame(-4, 12);
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.Circle, coordinatesNotFree1),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, coordinatesNotFree2),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, coordinatesNotFree3),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, coordinatesNotFree4),
            }, new List<Player>(), false);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, coordinatesNotFree1) }, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, coordinatesNotFree2) }, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, coordinatesNotFree3) }, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, coordinatesNotFree4) }, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenNoTilesOnGameAreAroundTile()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 0)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>(), false);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, new CoordinatesInGame(1, 7)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, new CoordinatesInGame(-1, 9)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, new CoordinatesInGame(0, 2)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Diamond, new CoordinatesInGame(0, -2)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(9, -3)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(3, -3)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(1, -4)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(2, -2)) }, null).Points);
    }

    [Fact]
    public void Return0WhenTileMakeBadColumn()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 0)),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 1)),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 2)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>(), false);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 3)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, -1)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.EightPointStar, new CoordinatesInGame(0, 3)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(0, -1)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -6)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -2)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileShape.Circle, new CoordinatesInGame(7, -6)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(7, -2)) }, null).Points);
    }

    [Fact]
    public void Return4When1TileMakeValidColumnWith3GameTiles()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Blue, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Orange, TileShape.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>(), false);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -6)) }, null).Points);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileShape.Square, new CoordinatesInGame(7, -2)) }, null).Points);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(7, -2)) }, null).Points);
        Assert.Equal(4, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -6)) }, null).Points);
        Assert.Equal(4, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileShape.Square, new CoordinatesInGame(7, -2)) }, null).Points);
        Assert.Equal(4, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(7, -2)) }, null).Points);
    }

    [Fact]
    public void Return0WhenTileMakeBadLine()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 0)),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(1, 0)),
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(2, 0)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>(), false);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(-1, 0)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(3, 0)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.EightPointStar, new CoordinatesInGame(-1, 0)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(3, 0)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(6, -4)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileShape.Circle, new CoordinatesInGame(6, -4)) }, null).Points);
        Assert.Equal(0, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(10, -4)) }, null).Points);
    }

    [Fact]
    public void Return4When1TileMakeValidLineWith3GameTiles()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileShape.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileShape.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>(), false);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(6, -4)) }, null).Points);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);
        Assert.Equal(4, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(6, -4)) }, null).Points);
        Assert.Equal(4, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);
        Assert.Equal(4, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);

    }

    [Fact]
    public void Return2When1GoodTileTouch1TileInGameOnSide()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileShape.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileShape.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>(), false);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(9, -5)) }, null).Points);
        Assert.Equal(2, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(9, -5)) }, null).Points);
    }

    [Fact]
    public void Return5When2TilesWithFirstTouchTileInGame()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileShape.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileShape.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>(), false);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(11, -4)) }, null).Points);
        Assert.Equal(5, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(11, -4)) }, null).Points);
    }

    [Fact]
    public void Return5When2TilesWithSecondTouchTileInGame()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileShape.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileShape.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>(), false);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(11, -4)), new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);
        Assert.Equal(5, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(11, -4)), new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(10, -4)) }, null).Points);
    }

    [Fact]
    public void Return7When3TilesRowTouch3GameTileLine()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileShape.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileShape.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>(), false);
        Assert.True(0 < UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(10, -5)), new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileShape.EightPointStar, new CoordinatesInGame(10, -3)) }, null).Points);
        Assert.Equal(7, UseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(10, -5)), new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileShape.EightPointStar, new CoordinatesInGame(10, -3)) }, null).Points);
    }

    [Fact]
    public void Return24WhenAQwirkleYellowLineTouchQwirklePurpleLine()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Diamond, new CoordinatesInGame(9, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(10, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(11, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Clover, new CoordinatesInGame(12, -4)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Circle, new CoordinatesInGame(8, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Diamond, new CoordinatesInGame(9, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.FourPointStar, new CoordinatesInGame(10, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.EightPointStar, new CoordinatesInGame(11, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(12, -3)),
            };
        Assert.True(0 < UseCase.GetPlayReturn(tilesTested, null).Points);
        Assert.Equal(6 + 6 + 2 * QwirklePoints, UseCase.GetPlayReturn(tilesTested, null).Points);
    }

    [Fact]
    public void Return0WhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Diamond, new CoordinatesInGame(9, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(10, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(11, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Clover, new CoordinatesInGame(12, -4)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Circle, new CoordinatesInGame(8, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.FourPointStar, new CoordinatesInGame(9, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Diamond, new CoordinatesInGame(10, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.EightPointStar, new CoordinatesInGame(11, -3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(12, -3)),
            };
        UseCase.GetPlayReturn(tilesTested, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInARow()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -4)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(8, -4)),
            };
        UseCase.GetPlayReturn(tilesTested, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInTheSameColumn()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Clover, new CoordinatesInGame(7, -1)),
                new TileOnBoard(TileColor.Purple, TileShape.Diamond, new CoordinatesInGame(7, 0)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(7, 1)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(7, -5)),
            };
        UseCase.GetPlayReturn(tilesTested, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInTheSameLine()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(-3, 7)),
                new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(-4, 7)),
                new TileOnBoard(TileColor.Purple, TileShape.Clover, new CoordinatesInGame(-1,7)),
                new TileOnBoard(TileColor.Purple, TileShape.Diamond, new CoordinatesInGame(0, 7)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(1, 7)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(-5, 7)),
            };
        var tilesTested2 = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(-5, 7)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(1, 7)),
            };
        UseCase.GetPlayReturn(tilesTested, null).Points.ShouldBe(0);
        UseCase.GetPlayReturn(tilesTested2, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return12When2TilesAreInTheSame4GameTilesColumn()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.FourPointStar, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileShape.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Purple, TileShape.Clover, new CoordinatesInGame(7, -1)),
                new TileOnBoard(TileColor.Purple, TileShape.Diamond, new CoordinatesInGame(7, 0)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(7, -2)),
            };
        var tilesTested2 = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileShape.EightPointStar, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Purple, TileShape.Circle, new CoordinatesInGame(7, -2)),
            };
        Assert.True(0 < UseCase.GetPlayReturn(tilesTested, null).Points);
        Assert.True(0 < UseCase.GetPlayReturn(tilesTested2, null).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.GetPlayReturn(tilesTested, null).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.GetPlayReturn(tilesTested2, null).Points);
    }

    [Fact]
    public void Return12When2TilesAreInTheSame4GameTilesLine()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileShape.Square, new CoordinatesInGame(14, 3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Clover, new CoordinatesInGame(16, 3)),
                new TileOnBoard(TileColor.Yellow, TileShape.EightPointStar, new CoordinatesInGame(13, 3)),
                new TileOnBoard(TileColor.Yellow, TileShape.FourPointStar, new CoordinatesInGame(17, 3)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileShape.Diamond, new CoordinatesInGame(15, 3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Circle, new CoordinatesInGame(18, 3)),
            };
        var tilesTested2 = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileShape.Diamond, new CoordinatesInGame(18, 3)),
                new TileOnBoard(TileColor.Yellow, TileShape.Circle, new CoordinatesInGame(15, 3)),
            };
        Assert.True(0 < UseCase.GetPlayReturn(tilesTested, null).Points);
        Assert.True(0 < UseCase.GetPlayReturn(tilesTested2, null).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.GetPlayReturn(tilesTested, null).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.GetPlayReturn(tilesTested2, null).Points);
    }

    [Fact]
    public void Return0WhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.FourPointStar, new CoordinatesInGame(7, 2)),
                new TileOnBoard(TileColor.Blue, TileShape.Diamond, new CoordinatesInGame(7, 1)),
                new TileOnBoard(TileColor.Green, TileShape.Circle, new CoordinatesInGame(7, -1)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(7, 0)),
            };
        UseCase.GetPlayReturn(tilesTested, null).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
    {
        UseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.FourPointStar, new CoordinatesInGame(2, 7)),
                new TileOnBoard(TileColor.Blue, TileShape.Diamond, new CoordinatesInGame(1, 7)),
                new TileOnBoard(TileColor.Green, TileShape.Circle, new CoordinatesInGame(-1, 7)),
            }, new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileShape.Circle, new CoordinatesInGame(0, 7)),
            };
        UseCase.GetPlayReturn(tilesTested, null).Points.ShouldBe(0);
    }
}
