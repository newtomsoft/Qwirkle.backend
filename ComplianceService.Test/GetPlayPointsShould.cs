﻿using Qwirkle.Core.CommonContext;
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
        public void ReturnNumberOfTilesWhenBoardIsEmptyAndTilesMakeRow()
        {
            ComplianceService.Board = new Board(1, new List<Tile>());
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 5)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(13, 69)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(12, 69)) }));
            Assert.Equal(1, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 5)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(13, 69)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(12, 69)) }));
        }

        [Fact]
        public void Return0WhenBoardIsEmptyAndTilesNotInRow()
        {
            ComplianceService.Board = new Board(1, new List<Tile>());
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 5)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(2, 4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(8, 12)), new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInBoard(9, 12)), new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(6, 12)) }));
        }

        [Fact]
        public void Return0WhenTilesAreInTheSamePlace()
        {
            ComplianceService.Board = new Board(1, new List<Tile>());

            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInBoard(6, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInBoard(6, -3)), new Tile(TileColor.Green, TileForm.Shape, new CoordinatesInBoard(5, -3)) }));

            ComplianceService.Board = new Board(1, new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)), });
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInBoard(6, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(6, -3)), new Tile(TileColor.Green, TileForm.Star4, new CoordinatesInBoard(6, -3)), new Tile(TileColor.Green, TileForm.Shape, new CoordinatesInBoard(5, -3)) }));
        }

        [Fact]
        public void Return2When1GoodTileIsAround1TileOnBoard()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(8, -3)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -3)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(8, -3)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -3)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
        }

        [Fact]
        public void Return0WhenTileIsOverOtherTileOnBoard()
        {
            var coordinatesNotFree1 = new CoordinatesInBoard(0, 0);
            var coordinatesNotFree2 = new CoordinatesInBoard(14, 28);
            var coordinatesNotFree3 = new CoordinatesInBoard(-7, 3);
            var coordinatesNotFree4 = new CoordinatesInBoard(-4, 12);
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree1),
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree2),
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree3),
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree4),
            });
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree1) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree2) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree3) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree4) }));
        }

        [Fact]
        public void Return0WhenNoTilesOnBoardAreAroundTile()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 7)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(-1, 9)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, 2)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -2)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(9, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(3, -3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(1, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(2, -2)) }));
        }

        [Fact]
        public void Return0WhenTileMakeBadColumn()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 1)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 2)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, -1)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInBoard(0, 3)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(0, -1)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -6)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInBoard(7, -6)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(7, -2)) }));
        }

        [Fact]
        public void Return4When1TileMakeValidColumnWith3BoardTiles()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -6)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -6)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
        }

        [Fact]
        public void Return0WhenTileMakeBadLine()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(2, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(-1, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(3, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInBoard(-1, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(3, 0)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(6, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInBoard(6, -4)) }));
            Assert.Equal(0, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void Return4When1TileMakeValidLineWith3BoardTiles()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -4)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -4)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.Equal(4, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));

        }

        [Fact]
        public void Return2When1GoodTileTouch1TileInBoardOnSide()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(9, -5)) }));
            Assert.Equal(2, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(9, -5)) }));
        }

        [Fact]
        public void Return5When2TilesWithFirstTouchTileInBoard()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(11, -4)) }));
            Assert.Equal(5, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(11, -4)) }));
        }

        [Fact]
        public void Return5When2TilesWithSecondTouchTileInBoard()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.Equal(5, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void Return7When3TilesRowTouch3BoardTileLine()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(10, -3)) }));
            Assert.Equal(7, ComplianceService.GetPlayPoints(new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(10, -3)) }));
        }

        [Fact]
        public void Return24WhenAQwirkleYellowLineTouchQwirklePurpleLine()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInBoard(9, -4)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(10, -4)),
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(11, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInBoard(12, -4)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInBoard(8, -3)),
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInBoard(9, -3)),
                new Tile(TileColor.Yellow, TileForm.Star4, new CoordinatesInBoard(10, -3)),
                new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(11, -3)),
                new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(12, -3)),
            };
            Assert.True(ComplianceService.GetPlayPoints(tilesTested) > 0);
            Assert.Equal(6 + 6 + 2 * QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInBoard(9, -4)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(10, -4)),
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(11, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInBoard(12, -4)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInBoard(8, -3)),
                new Tile(TileColor.Yellow, TileForm.Star4, new CoordinatesInBoard(9, -3)),
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInBoard(10, -3)),
                new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(11, -3)),
                new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(12, -3)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesAreNotInARow()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(8, -4)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesAreNotInTheSameColumn()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInBoard(7, -1)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInBoard(7, 0)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(7, 1)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(7, -5)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesAreNotInTheSameLine()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(-3, 7)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(-4, 7)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInBoard(-1,7)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInBoard(0, 7)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(1, 7)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(-5, 7)),
            };
            var tilesTested2 = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(-5, 7)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(1, 7)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested2));
        }

        [Fact]
        public void Return12When2TilesAreInTheSame4BoardTilesColumn()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInBoard(7, -1)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInBoard(7, 0)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(7, -2)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(7, -5)),
            };
            var tilesTested2 = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(7, -2)),
            };
            Assert.True(0 < ComplianceService.GetPlayPoints(tilesTested));
            Assert.True(0 < ComplianceService.GetPlayPoints(tilesTested2));
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested));
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested2));
        }

        [Fact]
        public void Return12When2TilesAreInTheSame4BoardTilesLine()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(14, 3)),
                new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(16, 3)),
                new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(13, 3)),
                new Tile(TileColor.Yellow, TileForm.Star4, new CoordinatesInBoard(17, 3)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInBoard(15, 3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInBoard(18, 3)),
            };
            var tilesTested2 = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Shape, new CoordinatesInBoard(18, 3)),
                new Tile(TileColor.Yellow, TileForm.Ring, new CoordinatesInBoard(15, 3)),
            };
            Assert.True(ComplianceService.GetPlayPoints(tilesTested) > 0);
            Assert.True(ComplianceService.GetPlayPoints(tilesTested2) > 0);
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested));
            Assert.Equal(6 + QWIRKLE_POINTS, ComplianceService.GetPlayPoints(tilesTested2));
        }

        [Fact]
        public void Return0WhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(7, 2)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(7, 1)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(7, -1)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(7, 0)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }

        [Fact]
        public void Return0WhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
        {
            ComplianceService.Board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(2, 7)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 7)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(-1, 7)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 7)),
            };
            Assert.Equal(0, ComplianceService.GetPlayPoints(tilesTested));
        }
    }
}