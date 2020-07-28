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
        public void ReturnTrueWhenBoardIsEmpty()
        {
            var board = new Board(1, new List<Tile>());
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 1)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(13, 69)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(27, 34)) }));
        } // todo equal

        [Fact]
        public void ReturnTrueWhenGoodTileIsAroundTileOnBoard()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(-1, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, 1)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -1)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(8, -3)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -3)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -4)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
        }// todo equal

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
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree1) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree2) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree3) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, coordinatesNotFree4) }));
        }

        [Fact]
        public void ReturnFalseWhenNoTilesOnBoardAreAroundTile()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 7)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(-1, 9)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, 2)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -2)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(9, -3)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(3, -3)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(1, -4)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(2, -2)) }));
        }

        [Fact]
        public void ReturnFalseWhenTileMakeBadColumn()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 1)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 2)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 3)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, -1)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInBoard(0, 3)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(0, -1)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -6)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInBoard(7, -6)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(7, -2)) }));
        }

        [Fact]
        public void ReturnTrueWhenTileMakeValidColumn()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(0, 1)),
                new Tile(TileColor.Blue, TileForm.Trefail, new CoordinatesInBoard(0, 2)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -5)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(7, -3)),
            });
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(0, 3)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Star8, new CoordinatesInBoard(0, -1)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(0, -1)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(7, -6)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(7, -2)) }));
        }

        [Fact]
        public void ReturnFalseWhenTileMakeBadLine()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(1, 0)),
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(2, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(-1, 0)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(3, 0)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Green, TileForm.Star8, new CoordinatesInBoard(-1, 0)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(3, 0)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(6, -4)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Red, TileForm.Ring, new CoordinatesInBoard(6, -4)) }));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Star4, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhenTileMakeValidLine()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(-1, 0)),
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(0, 0)),
                new Tile(TileColor.Blue, TileForm.Trefail, new CoordinatesInBoard(1, 0)),
                new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(-2, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Star8, new CoordinatesInBoard(2, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(2, 0)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(6, -4)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Red, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
        } // todo equal 

        [Fact]
        public void ReturnTrueWhen1GoodTileTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
            Assert.Equal(4, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhen2TilesWithFirstTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(11, -4)) }) > 0);
            Assert.Equal(5, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(11, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhen2TilesWithSecondTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }) > 0);
            Assert.Equal(5, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(11, -4)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)) }));
        }

        [Fact]
        public void ReturnTrueWhen3TilesWithSecondTouchTileInBoard()
        {
            var board = new Board(1, new List<Tile> {
            new Tile(TileColor.Green, TileForm.Square, new CoordinatesInBoard(7, -4)),
                new Tile(TileColor.Blue, TileForm.Square, new CoordinatesInBoard(8, -4)),
                new Tile(TileColor.Orange, TileForm.Square, new CoordinatesInBoard(9, -4)),
            });
            Assert.True(_complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(10, -3)) }) > 0);
            Assert.Equal(7, _complianceService.CanTilesBePlayed(board, new List<Tile> { new Tile(TileColor.Yellow, TileForm.Trefail, new CoordinatesInBoard(10, -5)), new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(10, -4)), new Tile(TileColor.Yellow, TileForm.Star8, new CoordinatesInBoard(10, -3)) }));
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
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested) > 0);
            Assert.Equal(18, _complianceService.CanTilesBePlayed(board, tilesTested)); //todo prendre en compte +6 du qwarkle
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
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested));
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
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested));
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
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested));
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
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested2));
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
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.True(0 < _complianceService.CanTilesBePlayed(board, tilesTested2));
            Assert.Equal(6, _complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.Equal(6, _complianceService.CanTilesBePlayed(board, tilesTested2));
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
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested) > 0);
            Assert.True(_complianceService.CanTilesBePlayed(board, tilesTested2) > 0);
            Assert.Equal(6, _complianceService.CanTilesBePlayed(board, tilesTested));
            Assert.Equal(6, _complianceService.CanTilesBePlayed(board, tilesTested2));
        }

        [Fact]
        public void FalseWhenTilesMakeColumnJoinedBy2ColumnsWithDifferentsTiles()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(7, 2)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(7, 1)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(7, -1)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(7, 0)),
            };
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested));
        }

        [Fact]
        public void FalseWhenTilesMakeLineJoinedBy2LinesWithDifferentsTiles()
        {
            var board = new Board(1, new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Star4, new CoordinatesInBoard(2, 7)),
                new Tile(TileColor.Blue, TileForm.Shape, new CoordinatesInBoard(1, 7)),
                new Tile(TileColor.Green, TileForm.Ring, new CoordinatesInBoard(-1, 7)),
            });
            var tilesTested = new List<Tile> {
                new Tile(TileColor.Blue, TileForm.Ring, new CoordinatesInBoard(0, 7)),
            };
            Assert.Equal(0, _complianceService.CanTilesBePlayed(board, tilesTested));
        }
    }
}
