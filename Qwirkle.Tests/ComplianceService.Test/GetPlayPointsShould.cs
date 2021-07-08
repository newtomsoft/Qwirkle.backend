using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.UsesCases;
using Qwirkle.Core.ValueObjects;
using System.Collections.Generic;
using Xunit;

namespace Qwirkle.Core.Tests
{
    public class GetPlayPointsShould
    {
        private const int QWIRKLE_POINTS = 6;
        private CoreUseCase CoreUseCase { get; } = new CoreUseCase(null);

        [Fact]
        public void ReturnNumberOfTilesWhenGameIsEmptyAndTilesMakeRow()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard>(), new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(1, 5)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(13, 69)), new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(12, 69)) }).Points);
            Assert.Equal(1, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(1, 5)) }).Points);
            Assert.Equal(2, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(13, 69)), new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(12, 69)) }).Points);
        }

        [Fact]
        public void Return0WhenGameIsEmptyAndTilesNotInRow()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard>(), new List<Player>());
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(1, 5)), new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(2, 4)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(8, 12)), new TileOnBoard(TileColor.Yellow, TileForm.Circle, new CoordinatesInGame(9, 12)), new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(6, 12)) }).Points);
        }

        [Fact]
        public void Return0WhenTilesAreInTheSamePlace()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard>(), new List<Player>());

            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileForm.FourPointStar, new CoordinatesInGame(6, -3)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileForm.FourPointStar, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileForm.Diamond, new CoordinatesInGame(5, -3)) }).Points);

            CoreUseCase.Game = new Game(1, new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)), }, new List<Player>());
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileForm.FourPointStar, new CoordinatesInGame(6, -3)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Circle, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileForm.FourPointStar, new CoordinatesInGame(6, -3)), new TileOnBoard(TileColor.Green, TileForm.Diamond, new CoordinatesInGame(5, -3)) }).Points);
        }

        [Fact]
        public void Return2When1GoodTileIsAround1TileOnGame()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(8, -3)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -3)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
            Assert.Equal(2, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(8, -3)) }).Points);
            Assert.Equal(2, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -3)) }).Points);
            Assert.Equal(2, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)) }).Points);
            Assert.Equal(2, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
        }

        [Fact]
        public void Return0WhenTileIsOverOtherTileOnGame()
        {
            var coordinatesNotFree1 = new CoordinatesInGame(0, 0);
            var coordinatesNotFree2 = new CoordinatesInGame(14, 28);
            var coordinatesNotFree3 = new CoordinatesInGame(-7, 3);
            var coordinatesNotFree4 = new CoordinatesInGame(-4, 12);
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.Circle, coordinatesNotFree1),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, coordinatesNotFree2),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, coordinatesNotFree3),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, coordinatesNotFree4),
            }, new List<Player>());
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, coordinatesNotFree1) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, coordinatesNotFree2) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, coordinatesNotFree3) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, coordinatesNotFree4) }).Points);
        }

        [Fact]
        public void Return0WhenNoTilesOnGameAreAroundTile()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 0)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, new CoordinatesInGame(1, 7)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, new CoordinatesInGame(-1, 9)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, new CoordinatesInGame(0, 2)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Diamond, new CoordinatesInGame(0, -2)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(9, -3)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(3, -3)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(1, -4)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(2, -2)) }).Points);
        }

        [Fact]
        public void Return0WhenTileMakeBadColumn()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 0)),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 1)),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 2)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 3)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, -1)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.EightPointStar, new CoordinatesInGame(0, 3)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(0, -1)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -6)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileForm.Circle, new CoordinatesInGame(7, -6)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(7, -2)) }).Points);
        }

        [Fact]
        public void Return4When1TileMakeValidColumnWith3GameTiles()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Blue, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Orange, TileForm.Square, new CoordinatesInGame(7, -3)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -6)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
            Assert.Equal(4, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -6)) }).Points);
            Assert.Equal(4, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
            Assert.Equal(4, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -2)) }).Points);
        }

        [Fact]
        public void Return0WhenTileMakeBadLine()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 0)),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(1, 0)),
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(2, 0)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(-1, 0)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(3, 0)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.EightPointStar, new CoordinatesInGame(-1, 0)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(3, 0)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(6, -4)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileForm.Circle, new CoordinatesInGame(6, -4)) }).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(10, -4)) }).Points);
        }

        [Fact]
        public void Return4When1TileMakeValidLineWith3GameTiles()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -4)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);
            Assert.Equal(4, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(6, -4)) }).Points);
            Assert.Equal(4, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Red, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);
            Assert.Equal(4, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);

        }

        [Fact]
        public void Return2When1GoodTileTouch1TileInGameOnSide()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(9, -5)) }).Points);
            Assert.Equal(2, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(9, -5)) }).Points);
        }

        [Fact]
        public void Return5When2TilesWithFirstTouchTileInGame()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(11, -4)) }).Points);
            Assert.Equal(5, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(11, -4)) }).Points);
        }

        [Fact]
        public void Return5When2TilesWithSecondTouchTileInGame()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(11, -4)), new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);
            Assert.Equal(5, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(11, -4)), new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)) }).Points);
        }

        [Fact]
        public void Return7When3TilesRowTouch3GameTileLine()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
            new TileOnBoard(TileColor.Green, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Blue, TileForm.Square, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Orange, TileForm.Square, new CoordinatesInGame(9, -4)),
            }, new List<Player>());
            Assert.True(0 < CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(10, -5)), new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileForm.EightPointStar, new CoordinatesInGame(10, -3)) }).Points);
            Assert.Equal(7, CoreUseCase.GetPlayReturn(new List<TileOnBoard> { new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(10, -5)), new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(10, -4)), new TileOnBoard(TileColor.Yellow, TileForm.EightPointStar, new CoordinatesInGame(10, -3)) }).Points);
        }

        [Fact]
        public void Return24WhenAQwirkleYellowLineTouchQwirklePurpleLine()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Diamond, new CoordinatesInGame(9, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(10, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(11, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Clover, new CoordinatesInGame(12, -4)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Circle, new CoordinatesInGame(8, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Diamond, new CoordinatesInGame(9, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.FourPointStar, new CoordinatesInGame(10, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.EightPointStar, new CoordinatesInGame(11, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(12, -3)),
            };
            Assert.True(0 < CoreUseCase.GetPlayReturn(tilesTested).Points);
            Assert.Equal(6 + 6 + 2 * QWIRKLE_POINTS, CoreUseCase.GetPlayReturn(tilesTested).Points);
        }

        [Fact]
        public void Return0WhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(8, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Diamond, new CoordinatesInGame(9, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(10, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(11, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Clover, new CoordinatesInGame(12, -4)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Circle, new CoordinatesInGame(8, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.FourPointStar, new CoordinatesInGame(9, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Diamond, new CoordinatesInGame(10, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.EightPointStar, new CoordinatesInGame(11, -3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(12, -3)),
            };
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested).Points);
        }

        [Fact]
        public void Return0WhenTilesAreNotInARow()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -4)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(8, -4)),
            };
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested).Points);
        }

        [Fact]
        public void Return0WhenTilesAreNotInTheSameColumn()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Clover, new CoordinatesInGame(7, -1)),
                new TileOnBoard(TileColor.Purple, TileForm.Diamond, new CoordinatesInGame(7, 0)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(7, 1)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(7, -5)),
            };
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested).Points);
        }

        [Fact]
        public void Return0WhenTilesAreNotInTheSameLine()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(-3, 7)),
                new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(-4, 7)),
                new TileOnBoard(TileColor.Purple, TileForm.Clover, new CoordinatesInGame(-1,7)),
                new TileOnBoard(TileColor.Purple, TileForm.Diamond, new CoordinatesInGame(0, 7)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(1, 7)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(-5, 7)),
            };
            var tilesTested2 = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(-5, 7)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(1, 7)),
            };
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested).Points);
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested2).Points);
        }

        [Fact]
        public void Return12When2TilesAreInTheSame4GameTilesColumn()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.Square, new CoordinatesInGame(7, -3)),
                new TileOnBoard(TileColor.Purple, TileForm.FourPointStar, new CoordinatesInGame(7, -4)),
                new TileOnBoard(TileColor.Purple, TileForm.Clover, new CoordinatesInGame(7, -1)),
                new TileOnBoard(TileColor.Purple, TileForm.Diamond, new CoordinatesInGame(7, 0)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(7, -2)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(7, -5)),
            };
            var tilesTested2 = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Purple, TileForm.EightPointStar, new CoordinatesInGame(7, -5)),
                new TileOnBoard(TileColor.Purple, TileForm.Circle, new CoordinatesInGame(7, -2)),
            };
            Assert.True(0 < CoreUseCase.GetPlayReturn(tilesTested).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(tilesTested2).Points);
            Assert.Equal(6 + QWIRKLE_POINTS, CoreUseCase.GetPlayReturn(tilesTested).Points);
            Assert.Equal(6 + QWIRKLE_POINTS, CoreUseCase.GetPlayReturn(tilesTested2).Points);
        }

        [Fact]
        public void Return12When2TilesAreInTheSame4GameTilesLine()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileForm.Square, new CoordinatesInGame(14, 3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Clover, new CoordinatesInGame(16, 3)),
                new TileOnBoard(TileColor.Yellow, TileForm.EightPointStar, new CoordinatesInGame(13, 3)),
                new TileOnBoard(TileColor.Yellow, TileForm.FourPointStar, new CoordinatesInGame(17, 3)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileForm.Diamond, new CoordinatesInGame(15, 3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Circle, new CoordinatesInGame(18, 3)),
            };
            var tilesTested2 = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Yellow, TileForm.Diamond, new CoordinatesInGame(18, 3)),
                new TileOnBoard(TileColor.Yellow, TileForm.Circle, new CoordinatesInGame(15, 3)),
            };
            Assert.True(0 < CoreUseCase.GetPlayReturn(tilesTested).Points);
            Assert.True(0 < CoreUseCase.GetPlayReturn(tilesTested2).Points);
            Assert.Equal(6 + QWIRKLE_POINTS, CoreUseCase.GetPlayReturn(tilesTested).Points);
            Assert.Equal(6 + QWIRKLE_POINTS, CoreUseCase.GetPlayReturn(tilesTested2).Points);
        }

        [Fact]
        public void Return0WhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.FourPointStar, new CoordinatesInGame(7, 2)),
                new TileOnBoard(TileColor.Blue, TileForm.Diamond, new CoordinatesInGame(7, 1)),
                new TileOnBoard(TileColor.Green, TileForm.Circle, new CoordinatesInGame(7, -1)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(7, 0)),
            };
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested).Points);
        }

        [Fact]
        public void Return0WhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
        {
            CoreUseCase.Game = new Game(1, new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.FourPointStar, new CoordinatesInGame(2, 7)),
                new TileOnBoard(TileColor.Blue, TileForm.Diamond, new CoordinatesInGame(1, 7)),
                new TileOnBoard(TileColor.Green, TileForm.Circle, new CoordinatesInGame(-1, 7)),
            }, new List<Player>());
            var tilesTested = new List<TileOnBoard> {
                new TileOnBoard(TileColor.Blue, TileForm.Circle, new CoordinatesInGame(0, 7)),
            };
            Assert.Equal(0, CoreUseCase.GetPlayReturn(tilesTested).Points);
        }
    }
}
