using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using System.Collections.Generic;
using Xunit;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class IsTileCanBePlayedAtThisCoordinateShould
    {
        private readonly ComplianceService _complianceService = new ComplianceService(null);

        [Fact]
        public void ReturnTrueWhenBoardIsEmpty()
        {
            var board = new Board(1, new List<Tile>());
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 1))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(13, 69))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(27, 34))));
        }

        [Fact]
        public void ReturnTrueWhenGoodTileIsAroundTileOnBoard()
        {
            var board = new Board(1, new List<Tile> { 
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(-1, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, 1))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -1))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(8, -3))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -3))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -2))));
        }

        [Fact]
        public void ReturnFalseWhenTileIsOverOtherTileOnBoard()
        {
            var coordinatesNotFree1 = new CoordinatesInBoard(0, 0);
            var coordinatesNotFree2 = new CoordinatesInBoard(14, 28);
            var coordinatesNotFree3 = new CoordinatesInBoard(-7, 3);
            var coordinatesNotFree4 = new CoordinatesInBoard(-4, 12);
            var board = new Board(1, new List<Tile> { 
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree1), 
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree2), 
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree3), 
                new Tile(TileColor.Blue, TileForm.Ring, coordinatesNotFree4),
            });
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree1)));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree2)));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree3)));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree4)));
        }

        [Fact]
        public void ReturnFalseWhenNoTilesOnBoardAreAroundTile()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 7))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(-1, 9))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, 2))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -2))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(9, -3))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(3, -3))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(1, -4))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(2, -2))));
        }

        [Fact]
        public void FalseWhenTileMakeBadColumn()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 1)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 2)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 3))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, -1))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInBoard(0, 3))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(0, -1))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -6))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -2))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInBoard(7, -6))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(7, -2))));
        }

        [Fact]
        public void TrueWhenTileMakeGoodColumn()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(0, 1)),
                new Tile(TileColor.Blue, TileForm.Trefail, new CoordinatesInBoard(0, 2)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(0, 3))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Star8, new CoordinatesInBoard(0, -1))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -1))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -6))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(7, -2))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -2))));
        }

        [Fact]
        public void FalseWhenTileMakeBadLine()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(2, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(-1, 0))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(3, 0))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInBoard(-1, 0))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(3, 0))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(6, -4))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(10, -4))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInBoard(6, -4))));
            Assert.False(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(10, -4))));
        }

        [Fact]
        public void TrueWhenTileMakeGoodLine()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(-1, 0)),
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Trefail, new CoordinatesInBoard(1, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(-2, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Star8, new CoordinatesInBoard(2, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(2, 0))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -4))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(10, -4))));
            Assert.True(_complianceService.IsTileCanBePlayedAtThisCoordinate(board, new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4))));
        }
    }
}
