namespace Qwirkle.Test;

public class GetPlayPointsShould
{
    private readonly Player _fakePlayer;
    private const int QwirklePoints = 6;
    private readonly Bag _fakeBag = new(0, new List<TileOnBag> { new(TileColor.Yellow, TileShape.EightPointStar) });
    private CoreService Service { get; } = new(null, null, null, null, null);

    public GetPlayPointsShould()
    {
        var fakeRack = Rack.From(new List<TileOnRack>());
        _fakePlayer = new Player(0, 0, 1, "", 0, 0, 0, fakeRack, true, false);
    }

    [Fact]
    public void Return1WhenGameIsEmptyAndPlayPossibleIs1Tile()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard>()), new List<Player>(), _fakeBag, false);
        var tilesToPlay = new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)) };
        var tilesOnPlayer = new List<TileOnRack> { new(0, TileColor.Blue, TileShape.Circle) };
        var rack = Rack.From(tilesOnPlayer);
        var player = new Player(0, 0, 0, "test", 1, 0, 0, rack, true, false);
        Service.Play(tilesToPlay, player, game).Move.Points.ShouldBe(1);
    }

    [Fact]
    public void Return2WhenGameIsEmptyAndTilesMakeRowOf2Tiles()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard>()), new List<Player>(), _fakeBag, false);

        var tilesToPlay = new HashSet<TileOnBoard>
            {
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(0, 1))
            };
        var tilesOnPlayer = new List<TileOnRack>
                {new(0, TileColor.Blue, TileShape.Circle), new(1, TileColor.Purple, TileShape.Circle)};
        var rack = Rack.From(tilesOnPlayer);
        var player = new Player(0, 0, 0, "test", 1, 0, 0, rack, true, false);
        Service.Play(tilesToPlay, player, game).Move.Points.ShouldBe(2);
    }

    [Fact]
    public void Return3WhenGameIsEmptyAndTilesMakeRowOf3Tiles()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard>()), new List<Player>(), _fakeBag, false);
        var tilesToPlay = new HashSet<TileOnBoard>
        {
            new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)),
            new(TileColor.Purple, TileShape.Circle, new Coordinate(0, 1)),
            new(TileColor.Yellow , TileShape.Circle, new Coordinate(0, 2))
        };
        var tilesOnPlayer = new List<TileOnRack>
            {new(0, TileColor.Blue, TileShape.Circle), new(1, TileColor.Purple, TileShape.Circle), new(2, TileColor.Yellow, TileShape.Circle)};
        var rack = Rack.From(tilesOnPlayer);
        var player = new Player(0, 0, 0, "test", 1, 0, 0, rack, true, false);
        Service.Play(tilesToPlay, player, game).Move.Points.ShouldBe(3);
    }

    [Fact]
    public void Return0WhenGameIsEmptyAndTilesNotInRow()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard>()), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(1, 5)), new(TileColor.Purple, TileShape.Circle, new Coordinate(2, 4)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(8, 12)), new(TileColor.Yellow, TileShape.Circle, new Coordinate(9, 12)), new(TileColor.Purple, TileShape.Circle, new Coordinate(6, 12)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return8WhenMoveGet2PointsAndEndGameIsSimulated()
    {
        var emptyBag = new Bag(0, new List<TileOnBag>());
        var game = new Game(1, Board.From(new HashSet<TileOnBoard>() { new(TileColor.Green, TileShape.Circle, new Coordinate(1, 0)) }), new List<Player>(), emptyBag, false);
        var tilesToPlay = new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)) };
        var tilesOnPlayer = new List<TileOnRack> { new(0, TileColor.Blue, TileShape.Circle) };
        var rack = Rack.From(tilesOnPlayer);
        var player = new Player(0, 0, 0, "test", 1, 0, 0, rack, true, false);

        var service = new CoreService(new NoRepository(), null, null, null, null);
        service.Play(tilesToPlay, player, game).Move.Points.ShouldBe(8);
    }

    [Fact]
    public void Return0WhenTilesAreInTheSamePlace()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard>()), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinate(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinate(6, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinate(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinate(6, -3)), new(TileColor.Green, TileShape.Diamond, new Coordinate(5, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);

        game = new Game(1, Board.From(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinate(7, -3)), }), new List<Player>(), false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinate(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinate(6, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Circle, new Coordinate(6, -3)), new(TileColor.Green, TileShape.FourPointStar, new Coordinate(6, -3)), new(TileColor.Green, TileShape.Diamond, new Coordinate(5, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return2When1GoodTileIsAround1TileOnGame()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinate(7, -3)) }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(8, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(2);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(6, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(2);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(7, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(2);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(2);
    }

    [Fact]
    public void Return0WhenTileIsOverOtherTileOnGame()
    {
        var coordinatesNotFree1 = new Coordinate(0, 0);
        var coordinatesNotFree2 = new Coordinate(14, 28);
        var coordinatesNotFree3 = new Coordinate(-7, 3);
        var coordinatesNotFree4 = new Coordinate(-4, 12);
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree1),
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree2),
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree3),
                new(TileColor.Blue, TileShape.Circle, coordinatesNotFree4),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree1) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree2) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree3) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, coordinatesNotFree4) }, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenNoTilesOnGameAreAroundTile()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)),
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -3)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinate(1, 7)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinate(-1, 9)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinate(0, 2)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Diamond, new Coordinate(0, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(9, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(3, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(1, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(2, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTileMakeBadColumn()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)),
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 1)),
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 2)),
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -5)),
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -3)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(0, -1)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.EightPointStar, new Coordinate(0, 3)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinate(0, -1)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinate(7, -6)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Red, TileShape.Circle, new Coordinate(7, -6)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return4When1TileMakeValidColumnWith3GameTiles()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -5)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(7, -3)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(7, -6)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(7, -6)) }, _fakePlayer, game).Move.Points.ShouldBe(4);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(4);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinate(7, -2)) }, _fakePlayer, game).Move.Points.ShouldBe(4);
    }

    [Fact]
    public void Return0WhenTileMakeBadLine()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 0)),
                new(TileColor.Blue, TileShape.Circle, new Coordinate(1, 0)),
                new(TileColor.Blue, TileShape.Circle, new Coordinate(2, 0)),
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Green, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Green, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(-1, 0)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Blue, TileShape.Circle, new Coordinate(3, 0)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.EightPointStar, new Coordinate(-1, 0)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinate(3, 0)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinate(6, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Green, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Red, TileShape.Circle, new Coordinate(6, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return4When1TileMakeValidLineWith3GameTiles()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(6, -4)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(6, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(4);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Red, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(4);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(4);

    }

    [Fact]
    public void Return2When1GoodTileTouch1TileInGameOnSide()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(9, -5)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(9, -5)) }, _fakePlayer, game).Move.Points.ShouldBe(2);
    }
    [Fact]
    public void Return1When1GoodTileTouch1TileInGameOnSide()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), false);
        Assert.True(0 < Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(9, -5)) }, _fakePlayer, game).Move.Points);
        Assert.Equal(2, Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(9, -5)) }, _fakePlayer, game).Move.Points);
    }
    [Fact]
    public void Return5When2TilesWithFirstTouchTileInGame()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(10, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinate(11, -4)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(10, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinate(11, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(5);
    }

    [Fact]
    public void Return5When2TilesWithSecondTouchTileInGame()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(11, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Purple, TileShape.Square, new Coordinate(11, -4)), new(TileColor.Yellow, TileShape.Square, new Coordinate(10, -4)) }, _fakePlayer, game).Move.Points.ShouldBe(5);
    }

    [Fact]
    public void Return7When3TilesRowTouch3GameTileLine()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
            new(TileColor.Green, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Blue, TileShape.Square, new Coordinate(8, -4)),
                new(TileColor.Orange, TileShape.Square, new Coordinate(9, -4)),
            }), new List<Player>(), _fakeBag, false);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinate(10, -5)), new(TileColor.Yellow, TileShape.Square, new Coordinate(10, -4)), new(TileColor.Yellow, TileShape.EightPointStar, new Coordinate(10, -3)) }, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(new HashSet<TileOnBoard> { new(TileColor.Yellow, TileShape.Clover, new Coordinate(10, -5)), new(TileColor.Yellow, TileShape.Square, new Coordinate(10, -4)), new(TileColor.Yellow, TileShape.EightPointStar, new Coordinate(10, -3)) }, _fakePlayer, game).Move.Points.ShouldBe(7);
    }

    [Fact]
    public void Return24WhenAQwirkleYellowLineTouchQwirklePurpleLine()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(8, -4)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinate(9, -4)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(10, -4)),
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(11, -4)),
                new(TileColor.Purple, TileShape.Clover, new Coordinate(12, -4)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinate(7, -3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinate(8, -3)),
                new(TileColor.Yellow, TileShape.Diamond, new Coordinate(9, -3)),
                new(TileColor.Yellow, TileShape.FourPointStar, new Coordinate(10, -3)),
                new(TileColor.Yellow, TileShape.EightPointStar, new Coordinate(11, -3)),
                new(TileColor.Yellow, TileShape.Clover, new Coordinate(12, -3)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBeGreaterThan(0);
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(6 + 6 + 2 * QwirklePoints);
    }

    [Fact]
    public void Return0WhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinate(7, -4)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(8, -4)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinate(9, -4)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(10, -4)),
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(11, -4)),
                new(TileColor.Purple, TileShape.Clover, new Coordinate(12, -4)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinate(7, -3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinate(8, -3)),
                new(TileColor.Yellow, TileShape.FourPointStar, new Coordinate(9, -3)),
                new(TileColor.Yellow, TileShape.Diamond, new Coordinate(10, -3)),
                new(TileColor.Yellow, TileShape.EightPointStar, new Coordinate(11, -3)),
                new(TileColor.Yellow, TileShape.Clover, new Coordinate(12, -3)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInARow()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinate(7, -4)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinate(7, -3)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(8, -4)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInTheSameColumn()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinate(7, -3)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(7, -4)),
                new(TileColor.Purple, TileShape.Clover, new Coordinate(7, -1)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinate(7, 0)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(7, 1)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(7, -5)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesAreNotInTheSameLine()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.Square, new Coordinate(-3, 7)),
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(-4, 7)),
                new(TileColor.Purple, TileShape.Clover, new Coordinate(-1,7)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinate(0, 7)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(1, 7)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(-5, 7)),
            };
        var tilesTested2 = new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(-5, 7)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(1, 7)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(0);
        Service.Play(tilesTested2, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return12When2TilesAreInTheSame4GameTilesColumn()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.FourPointStar, new Coordinate(7, -4)),
                new(TileColor.Purple, TileShape.Square, new Coordinate(7, -3)),
                new(TileColor.Purple, TileShape.Clover, new Coordinate(7, -1)),
                new(TileColor.Purple, TileShape.Diamond, new Coordinate(7, 0)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.Circle, new Coordinate(7, -5)),
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(7, -2)),
            };
        var tilesTested2 = new HashSet<TileOnBoard> {
                new(TileColor.Purple, TileShape.EightPointStar, new Coordinate(7, -5)),
                new(TileColor.Purple, TileShape.Circle, new Coordinate(7, -2)),
            };
        Assert.True(0 < Service.Play(tilesTested, _fakePlayer, game).Move.Points);
        Assert.True(0 < Service.Play(tilesTested2, _fakePlayer, game).Move.Points);
        Assert.Equal(6 + QwirklePoints, Service.Play(tilesTested, _fakePlayer, game).Move.Points);
        Assert.Equal(6 + QwirklePoints, Service.Play(tilesTested2, _fakePlayer, game).Move.Points);
    }

    [Fact]
    public void Return12When2TilesAreInTheSame4GameTilesLine()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Square, new Coordinate(14, 3)),
                new(TileColor.Yellow, TileShape.Clover, new Coordinate(16, 3)),
                new(TileColor.Yellow, TileShape.EightPointStar, new Coordinate(13, 3)),
                new(TileColor.Yellow, TileShape.FourPointStar, new Coordinate(17, 3)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Diamond, new Coordinate(15, 3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinate(18, 3)),
            };
        var tilesTested2 = new HashSet<TileOnBoard> {
                new(TileColor.Yellow, TileShape.Diamond, new Coordinate(18, 3)),
                new(TileColor.Yellow, TileShape.Circle, new Coordinate(15, 3)),
            };
        Assert.True(0 < Service.Play(tilesTested, _fakePlayer, game).Move.Points);
        Assert.True(0 < Service.Play(tilesTested2, _fakePlayer, game).Move.Points);
        Assert.Equal(6 + QwirklePoints, Service.Play(tilesTested, _fakePlayer, game).Move.Points);
        Assert.Equal(6 + QwirklePoints, Service.Play(tilesTested2, _fakePlayer, game).Move.Points);
    }

    [Fact]
    public void Return0WhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.FourPointStar, new Coordinate(7, 2)),
                new(TileColor.Blue, TileShape.Diamond, new Coordinate(7, 1)),
                new(TileColor.Green, TileShape.Circle, new Coordinate(7, -1)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinate(7, 0)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(0);
    }

    [Fact]
    public void Return0WhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
    {
        var game = new Game(1, Board.From(new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.FourPointStar, new Coordinate(2, 7)),
                new(TileColor.Blue, TileShape.Diamond, new Coordinate(1, 7)),
                new(TileColor.Green, TileShape.Circle, new Coordinate(-1, 7)),
            }), new List<Player>(), _fakeBag, false);
        var tilesTested = new HashSet<TileOnBoard> {
                new(TileColor.Blue, TileShape.Circle, new Coordinate(0, 7)),
            };
        Service.Play(tilesTested, _fakePlayer, game).Move.Points.ShouldBe(0);
    }
}
