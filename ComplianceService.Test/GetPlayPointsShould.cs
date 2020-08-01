using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Services;
using System.Collections.Generic;
using Xunit;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class GetPlayPointsShould
    {
        private const int QWIRKLE_POINTS = 6;
        private ComplianceService ComplianceService { get; } = new ComplianceService(null);

        [Fact]
        public void ReturnNumberOfTilesWhenGameIsEmptyAndTilesMakeRow()
        {
            ComplianceService.Game = new Game(1, new List<Tile>(), new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(1, 5)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(13, 69)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(12, 69)) }));
            Assert.Equal(1, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(1, 5)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(13, 69)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(12, 69)) }));
        }

        [Fact]
        public void Return0WhenGameIsEmptyAndTilesNotInRow()
        {
            ComplianceService.Game = new Game(1, new List<Tile>(), new List<Player>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(1, 5)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(2, 4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(8, 12)), new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInGame(9, 12)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(6, 12)) }));
        }

        [Fact]
        public void Return0WhenTilesAreInTheSamePlace()
        {
            ComplianceService.Game = new Game(1, new List<Tile>(), new List<Player>());

            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInGame(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInGame(6, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInGame(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInGame(6, -3)), new Tile(TileColor.Green, TileForm.Shape, new CoordinatesInGame(5, -3)) }));

            ComplianceService.Game = new Game(1, new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)), }, new List<Player>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInGame(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInGame(6, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInGame(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInGame(6, -3)), new Tile(TileColor.Green, TileForm.Shape, new CoordinatesInGame(5, -3)) }));
        }

        [Fact]
        public void Return2When1GoodTileIsAround1TileOnGame()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(8, -3)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -3)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -2)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(8, -3)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -3)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -2)) }));
        }

        [Fact]
        public void Return0WhenTileIsOverOtherTileOnGame()
        {
            var coordinatesNotFree1 = new CoordinatesInGame(0, 0);
            var coordinatesNotFree2 = new CoordinatesInGame(14, 28);
            var coordinatesNotFree3 = new CoordinatesInGame(-7, 3);
            var coordinatesNotFree4 = new CoordinatesInGame(-4, 12);
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree1),
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree2),
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree3),
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree4),
            }, new List<Player>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree1) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree2) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree3) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree4) }));
        }

        [Fact]
        public void Return0WhenNoTilesOnGameAreAroundTile()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInGame(1, 7)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInGame(-1, 9)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInGame(0, 2)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInGame(0, -2)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(9, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(3, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(1, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(2, -2)) }));
        }

        [Fact]
        public void Return0WhenTileMakeBadColumn()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 1)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 2)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -5)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, -1)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInGame(0, 3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(0, -1)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -6)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -2)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInGame(7, -6)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(7, -2)) }));
        }

        [Fact]
        public void Return4When1TileMakeValidColumnWith3GameTiles()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -5)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -6)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInGame(7, -2)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -2)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -6)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInGame(7, -2)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -2)) }));
        }

        [Fact]
        public void Return0WhenTileMakeBadLine()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(1, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(2, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(-1, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(3, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInGame(-1, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(3, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(6, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(10, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInGame(6, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(10, -4)) }));
        }

        [Fact]
        public void Return4When1TileMakeValidLineWith3GameTiles()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -4)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInGame(10, -4)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -4)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInGame(10, -4)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }));

        }

        [Fact]
        public void Return2When1GoodTileTouch1TileInGameOnSide()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(9, -5)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(9, -5)) }));
        }

        [Fact]
        public void Return5When2TilesWithFirstTouchTileInGame()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(11, -4)) }));
            Assert.Equal(5, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(11, -4)) }));
        }

        [Fact]
        public void Return5When2TilesWithSecondTouchTileInGame()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }));
            Assert.Equal(5, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }));
        }

        [Fact]
        public void Return7When3TilesRowTouch3GameTileLine()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInGame(10, -3)) }));
            Assert.Equal(7, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInGame(10, -3)) }));
        }

        [Fact]
        public void Return24WhenAQwirkleYellowLineTouchQwirklePurpleLine()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInGame(9, -4)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(10, -4)),
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(11, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInGame(12, -4)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInGame(8, -3)),
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInGame(9, -3)),
                new Tile(TileColor.Yellow, TileForm.Star4, new CoordinatesInGame(10, -3)),
                new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInGame(11, -3)),
                new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(12, -3)),
            };
            Assert.True(ComplianceService.GetPlayPoints(tilesTested) > 0);
            Assert.Equal(6 + 6 + 2 * QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(8, -4)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInGame(9, -4)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(10, -4)),
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(11, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInGame(12, -4)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInGame(8, -3)),
                new Tile(TileColor.Yellow, TileForm.Star4, new CoordinatesInGame(9, -3)),
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInGame(10, -3)),
                new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInGame(11, -3)),
                new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(12, -3)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesAreNotInARow()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -3)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(8, -4)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesAreNotInTheSameColumn()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -3)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInGame(7, -1)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInGame(7, 0)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(7, 1)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(7, -5)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesAreNotInTheSameLine()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(-3, 7)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(-4, 7)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInGame(-1,7)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInGame(0, 7)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(1, 7)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(-5, 7)),
            };
            var tilesTested2 = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(-5, 7)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(1, 7)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested2));
        }

        [Fact]
        public void Return12When2TilesAreInTheSame4GameTilesColumn()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -3)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInGame(7, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInGame(7, -1)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInGame(7, 0)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(7, -2)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(7, -5)),
            };
            var tilesTested2 = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInGame(7, -5)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInGame(7, -2)),
            };
            Assert.True(0 < ComplianceService.GetPlayPoints(tilesTested));
            Assert.True(0 < ComplianceService.GetPlayPoints(tilesTested2));
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested));
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested2));
        }

        [Fact]
        public void Return12When2TilesAreInTheSame4GameTilesLine()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(14, 3)),
                new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInGame(16, 3)),
                new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInGame(13, 3)),
                new Tile(TileColor.Yellow, TileForm.Star4, new CoordinatesInGame(17, 3)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInGame(15, 3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInGame(18, 3)),
            };
            var tilesTested2 = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInGame(18, 3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInGame(15, 3)),
            };
            Assert.True(ComplianceService.GetPlayPoints(tilesTested) > 0);
            Assert.True(ComplianceService.GetPlayPoints(tilesTested2) > 0);
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested));
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested2));
        }

        [Fact]
        public void Return0WhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInGame(7, 2)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInGame(7, 1)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInGame(7, -1)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(7, 0)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
        {
            ComplianceService.Game = new Game(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInGame(2, 7)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInGame(1, 7)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInGame(-1, 7)),
            }, new List<Player>());
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInGame(0, 7)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }
    }
}
