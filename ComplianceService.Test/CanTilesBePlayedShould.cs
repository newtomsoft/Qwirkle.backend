using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Services;
using System.Collections.Generic;
using Xunit;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class CanTilesBePlayedShould
    {
        private readonly ComplianceService _complianceService = new ComplianceService(null);

        [Fact]
        public void ReturnTrueWhen1GoodTileTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhen2TilesWithFirstTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(11, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhen2TilesWithSecondTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhen3TilesWithSecondTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(10, -3)) }));
        }

        [Fact]
        public void ReturnTrueWhenAQwirkleYellowTouchQwirklePurple()
        {
            var board = new Board(1, new List<Tile> {
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
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested));
        }

        [Fact]
        public void ReturnFalseWhenAQwirkleYellowTouchQwirklePurpleButNotInTheSameOrder()
        {
            var board = new Board(1, new List<Tile> {
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
            Assert.False(_complianceService.CanTilesBePlayed(board, tilesTested));
        }

        [Fact]
        public void ReturnFalseWhenTilesAreNotInARow()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(8, -4)),
            };
            Assert.False(_complianceService.CanTilesBePlayed(board, tilesTested));
        }

        [Fact]
        public void ReturnFalseWhenTilesAreNotInTheSameColumn()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -3)),
                new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Purple, TileForm.Trefail, new CoordinatesInBoard(7, -1)),
                new Tile(TileColor.Purple, TileForm.Shape, new CoordinatesInBoard(7, 0)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Purple, TileForm.Star8, new CoordinatesInBoard(7, 1)),
                new Tile(TileColor.Purple, TileForm.Ring, new CoordinatesInBoard(7, -5)),
            };
            Assert.False(_complianceService.CanTilesBePlayed(board, tilesTested));
        }

        [Fact]
        public void ReturnFalseWhenTilesAreNotInTheSameLine()
        {
            var board = new Board(1, new List<Tile> {
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
            Assert.False(_complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.False(_complianceService.CanTilesBePlayed(board, tilesTested2));
        }

        [Fact]
        public void ReturnTrueWhenTilesAreInTheSameColumn()
        {
            var board = new Board(1, new List<Tile> {
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
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested2));
        }

        [Fact]
        public void ReturnTrueWhenTilesAreInTheSameLine()
        {
            var board = new Board(1, new List<Tile> {
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
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested2));
        }

        [Fact]
        public void FalseWhenTilesMakeBadColumn()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(7, 2)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(7, 1)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(7, -1)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 3)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, -1)),

                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -6)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -2)),
            };
            Assert.False(_complianceService.CanTilesBePlayed(board, tilesTested));
        }
    }
}
