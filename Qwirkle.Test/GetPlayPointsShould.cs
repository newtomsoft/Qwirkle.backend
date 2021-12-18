namespace Qwirkle.Test;

public class GetPlayPointsShould
{
    private const int QwirklePoints = 6;
    private CoreUseCase UseCase { get; } = new(null, null, null);

    [Fact]
    public void ReturnNumberOfTilesWhenGameIsEmptyAndTilesMakeRow()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard>()), new List<Player>(), false);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(1, 5)) }, null, game).Points.ShouldBe(1);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(13, 69)), new(TileColor.Purple, TileShape.Circle, new Coordinates(12, 69)) }, null, game).Points.ShouldBe(2);
    }

    [Fact]
    public void Return0WhenGameIsEmptyAndTilesNotInRow()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard>()), new List<Player>(), false);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(1, 5)), new(TileColor.Purple, TileShape.Circle, new Coordinates(2, 4)) }, null, game).Points.ShouldBe(0);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(8, 12)), new(TileColor.Yellow, TileShape.Circle, new Coordinates(9, 12)), new(TileColor.Purple, TileShape.Circle, new Coordinates(6, 12)) }, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreInTheSamePlace()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard>()), new List<Player>(), false);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinates(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinates(6, -3)) }, null, game).Points.ShouldBe(0);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinates(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinates(6, -3)), new(TileColor.Green, TileShape.Diamond, new Coordinates(5, -3)) }, null, game).Points.ShouldBe(0);

        game = new Game(1, Board.From(new List<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinates(7, -3)), }), new List<Player>(), false);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinates(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinates(6, -3)) }, null, game).Points.ShouldBe(0);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinates(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinates(6, -3)), new(TileColor.Green, TileShape.Diamond, new Coordinates(5, -3)) }, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return2When1GoodTileIsAround1TileOnGame()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -3)),
            }), new List<Player>(), false);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(8, -3)) }, null, game).Points.ShouldBe(2);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(6, -3)) }, null, game).Points.ShouldBe(2);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(7, -4)) }, null, game).Points.ShouldBe(2);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(7, -2)) }, null, game).Points.ShouldBe(2);
    }

    [Fact]
    public void Return0WhenTileIsOverOtherTileOnGame()
    {
        var coordinatesNotFree1 = new Coordinates(0, 0);
        var coordinatesNotFree2 = new Coordinates(14, 28);
        var coordinatesNotFree3 = new Coordinates(-7, 3);
        var coordinatesNotFree4 = new Coordinates(-4, 12);
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree1),
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree2),
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree3),
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree4),
            }), new List<Player>(), false);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree1) }, null, game).Points.ShouldBe(0);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree2) }, null, game).Points.ShouldBe(0);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree3) }, null, game).Points.ShouldBe(0);
        UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree4) }, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenNoTilesOnGameAreAroundTile()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 0)),
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -3)),
            }), new List<Player>(), false);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinates(1, 7)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinates(-1, 9)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinates(0, 2)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinates(0, -2)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(9, -3)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(3, -3)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(1, -4)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(2, -2)) }, null, game).Points);
    }

    [Fact]
    public void Return0WhenTileMakeBadColumn()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 0)),
                new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 1)),
                new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 2)),
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -5)),
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -3)),
            }), new List<Player>(), false);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 3)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(0, -1)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.EightPointStar, new Coordinates(0, 3)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinates(0, -1)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinates(7, -6)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinates(7, -2)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Red, TileShape.Circle, new Coordinates(7, -6)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(7, -2)) }, null, game).Points);
    }

    [Fact]
    public void Return4When1TileMakeValidColumnWith3GameTiles()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -5)),
                new(TileColor.Blue, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinates(7, -3)),
            }), new List<Player>(), false);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(7, -6)) }, null, game).Points);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinates(7, -2)) }, null, game).Points);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinates(7, -2)) }, null, game).Points);
        Assert.Equal(4, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(7, -6)) }, null, game).Points);
        Assert.Equal(4, UseCase.Play(new List<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinates(7, -2)) }, null, game).Points);
        Assert.Equal(4, UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinates(7, -2)) }, null, game).Points);
    }

    [Fact]
    public void Return0WhenTileMakeBadLine()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 0)),
                new(TileColor.Blue, TileShape.Circle, new Coordinates(1, 0)),
                new(TileColor.Blue, TileShape.Circle, new Coordinates(2, 0)),
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Green, TileShape.Square, new Coordinates(8, -4)),
                new(TileColor.Green, TileShape.Square, new Coordinates(9, -4)),
            }), new List<Player>(), false);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(-1, 0)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinates(3, 0)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.EightPointStar, new Coordinates(-1, 0)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinates(3, 0)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinates(6, -4)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Red, TileShape.Circle, new Coordinates(6, -4)) }, null, game).Points);
        Assert.Equal(0, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(10, -4)) }, null, game).Points);
    }

    [Fact]
    public void Return4When1TileMakeValidLineWith3GameTiles()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinates(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinates(9, -4)),
            }), new List<Player>(), false);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(6, -4)) }, null, game).Points);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);
        Assert.Equal(4, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(6, -4)) }, null, game).Points);
        Assert.Equal(4, UseCase.Play(new List<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);
        Assert.Equal(4, UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);

    }

    [Fact]
    public void Return2When1GoodTileTouch1TileInGameOnSide()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinates(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinates(9, -4)),
            }), new List<Player>(), false);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(9, -5)) }, null, game).Points);
        Assert.Equal(2, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(9, -5)) }, null, game).Points);
    }

    [Fact]
    public void Return5When2TilesWithFirstTouchTileInGame()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinates(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinates(9, -4)),
            }), new List<Player>(), false);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(10, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinates(11, -4)) }, null, game).Points);
        Assert.Equal(5, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(10, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinates(11, -4)) }, null, game).Points);
    }

    [Fact]
    public void Return5When2TilesWithSecondTouchTileInGame()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinates(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinates(9, -4)),
            }), new List<Player>(), false);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(11, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);
        Assert.Equal(5, UseCase.Play(new List<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinates(11, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinates(10, -4)) }, null, game).Points);
    }

    [Fact]
    public void Return7When3TilesRowTouch3GameTileLine()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinates(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinates(9, -4)),
            }), new List<Player>(), false);
        Assert.True(0 < UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinates(10, -5)), new(TileColor.Yellow, TileShape.Square, new Coordinates(10, -4)), new(TileColor.Yellow, TileShape.EightPointStar, new Coordinates(10, -3)) }, null, game).Points);
        Assert.Equal(7, UseCase.Play(new List<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinates(10, -5)), new(TileColor.Yellow, TileShape.Square, new Coordinates(10, -4)), new(TileColor.Yellow, TileShape.EightPointStar, new Coordinates(10, -3)) }, null, game).Points);
    }

    [Fact]
    public void Return24WhenAQwirkleYellowLineTouchQwirklePurpleLine()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(8, -4)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinates(9, -4)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(10, -4)),
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(11, -4)),
                new(TileColor.Purple, TileShape.Clover, new Coordinates(12, -4)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinates(7, -3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinates(8, -3)),
                new(TileColor.Yellow, TileShape.Diamond, new Coordinates(9, -3)),
                new(TileColor.Yellow, TileShape.FourPointStar, new Coordinates(10, -3)),
                new(TileColor.Yellow, TileShape.EightPointStar, new Coordinates(11, -3)),
                new(TileColor.Yellow, TileShape.Clover, new Coordinates(12, -3)),
            };
        Assert.True(0 < UseCase.Play(tilesTested, null, game).Points);
        Assert.Equal(6 + 6 + 2 * QwirklePoints, UseCase.Play(tilesTested, null, game).Points);
    }

    [Fact]
    public void Return0WhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinates(7, -4)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(8, -4)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinates(9, -4)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(10, -4)),
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(11, -4)),
                new(TileColor.Purple, TileShape.Clover, new Coordinates(12, -4)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinates(7, -3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinates(8, -3)),
                new(TileColor.Yellow, TileShape.FourPointStar, new Coordinates(9, -3)),
                new(TileColor.Yellow, TileShape.Diamond, new Coordinates(10, -3)),
                new(TileColor.Yellow, TileShape.EightPointStar, new Coordinates(11, -3)),
                new(TileColor.Yellow, TileShape.Clover, new Coordinates(12, -3)),
            };
        UseCase.Play(tilesTested, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInARow()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinates(7, -4)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinates(7, -3)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(8, -4)),
            };
        UseCase.Play(tilesTested, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInTheSameColumn()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinates(7, -3)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(7, -4)),
                new(TileColor.Purple, TileShape.Clover, new Coordinates(7, -1)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinates(7, 0)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(7, 1)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(7, -5)),
            };
        UseCase.Play(tilesTested, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInTheSameLine()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinates(-3, 7)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(-4, 7)),
                new(TileColor.Purple, TileShape.Clover, new Coordinates(-1,7)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinates(0, 7)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(1, 7)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(-5, 7)),
            };
        var tilesTested2 = new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(-5, 7)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(1, 7)),
            };
        UseCase.Play(tilesTested, null, game).Points.ShouldBe(0);
        UseCase.Play(tilesTested2, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return12When2TilesAreInTheSame4GameTilesColumn()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinates(7, -4)),
                new(TileColor.Purple, TileShape.Square, new Coordinates(7, -3)),
                new(TileColor.Purple, TileShape.Clover, new Coordinates(7, -1)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinates(7, 0)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.Circle, new Coordinates(7, -5)),
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(7, -2)),
            };
        var tilesTested2 = new List<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinates(7, -5)),
                new(TileColor.Purple, TileShape.Circle, new Coordinates(7, -2)),
            };
        Assert.True(0 < UseCase.Play(tilesTested, null, game).Points);
        Assert.True(0 < UseCase.Play(tilesTested2, null, game).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.Play(tilesTested, null, game).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.Play(tilesTested2, null, game).Points);
    }

    [Fact]
    public void Return12When2TilesAreInTheSame4GameTilesLine()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinates(14, 3)),
                new(TileColor.Yellow, TileShape.Clover, new Coordinates(16, 3)),
                new(TileColor.Yellow, TileShape.EightPointStar, new Coordinates(13, 3)),
                new(TileColor.Yellow, TileShape.FourPointStar, new Coordinates(17, 3)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Diamond, new Coordinates(15, 3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinates(18, 3)),
            };
        var tilesTested2 = new List<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Diamond, new Coordinates(18, 3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinates(15, 3)),
            };
        Assert.True(0 < UseCase.Play(tilesTested, null, game).Points);
        Assert.True(0 < UseCase.Play(tilesTested2, null, game).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.Play(tilesTested, null, game).Points);
        Assert.Equal(6 + QwirklePoints, UseCase.Play(tilesTested2, null, game).Points);
    }

    [Fact]
    public void Return0WhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.FourPointStar, new Coordinates(7, 2)),
                new(TileColor.Blue, TileShape.Diamond, new Coordinates(7, 1)),
                new(TileColor.Green, TileShape.Circle, new Coordinates(7, -1)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinates(7, 0)),
            };
        UseCase.Play(tilesTested, null, game).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
    {
        var game = new Game(1, Board.From(new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.FourPointStar, new Coordinates(2, 7)),
                new(TileColor.Blue, TileShape.Diamond, new Coordinates(1, 7)),
                new(TileColor.Green, TileShape.Circle, new Coordinates(-1, 7)),
            }), new List<Player>(), false);
        var tilesTested = new List<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinates(0, 7)),
            };
        UseCase.Play(tilesTested, null, game).Points.ShouldBe(0);
    }
}
