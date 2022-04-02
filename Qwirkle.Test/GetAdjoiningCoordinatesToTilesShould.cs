// ReSharper disable UnusedMember.Local
namespace Qwirkle.Test;

public class GetAdjoiningCoordinatesToTilesShould
{
    #region private
    readonly Coordinate _coord00 = Coordinate.From(0, 0);
    readonly Coordinate _coord01 = Coordinate.From(0, 1);
    readonly Coordinate _coord02 = Coordinate.From(0, 2);
    readonly Coordinate _coord03 = Coordinate.From(0, 3);
    readonly Coordinate _coord04 = Coordinate.From(0, 4);
    readonly Coordinate _coord05 = Coordinate.From(0, 5);
    readonly Coordinate _coord06 = Coordinate.From(0, 6);
    readonly Coordinate _coord10 = Coordinate.From(1, 0);
    readonly Coordinate _coord11 = Coordinate.From(1, 1);
    readonly Coordinate _coord12 = Coordinate.From(1, 2);
    readonly Coordinate _coord13 = Coordinate.From(1, 3);
    readonly Coordinate _coord14 = Coordinate.From(1, 4);
    readonly Coordinate _coord15 = Coordinate.From(1, 5);
    readonly Coordinate _coord16 = Coordinate.From(1, 6);
    readonly Coordinate _coord20 = Coordinate.From(2, 0);
    readonly Coordinate _coord21 = Coordinate.From(2, 1);
    readonly Coordinate _coord22 = Coordinate.From(2, 2);
    readonly Coordinate _coord23 = Coordinate.From(2, 3);
    readonly Coordinate _coord24 = Coordinate.From(2, 4);
    readonly Coordinate _coord25 = Coordinate.From(2, 5);
    readonly Coordinate _coord26 = Coordinate.From(2, 6);
    readonly Coordinate _coord30 = Coordinate.From(3, 0);
    readonly Coordinate _coord31 = Coordinate.From(3, 1);
    readonly Coordinate _coord32 = Coordinate.From(3, 2);
    readonly Coordinate _coord33 = Coordinate.From(3, 3);
    readonly Coordinate _coord34 = Coordinate.From(3, 4);
    readonly Coordinate _coord35 = Coordinate.From(3, 5);
    readonly Coordinate _coord36 = Coordinate.From(3, 6);
    readonly Coordinate _coord40 = Coordinate.From(4, 0);
    readonly Coordinate _coord41 = Coordinate.From(4, 1);
    readonly Coordinate _coord42 = Coordinate.From(4, 2);
    readonly Coordinate _coord43 = Coordinate.From(4, 3);
    readonly Coordinate _coord44 = Coordinate.From(4, 4);
    readonly Coordinate _coord45 = Coordinate.From(4, 5);
    readonly Coordinate _coord46 = Coordinate.From(4, 6);
    readonly Coordinate _coord50 = Coordinate.From(5, 0);
    readonly Coordinate _coord51 = Coordinate.From(5, 1);
    readonly Coordinate _coord52 = Coordinate.From(5, 2);
    readonly Coordinate _coord53 = Coordinate.From(5, 3);
    readonly Coordinate _coord54 = Coordinate.From(5, 4);
    readonly Coordinate _coord55 = Coordinate.From(5, 5);
    readonly Coordinate _coord56 = Coordinate.From(5, 6);
    readonly Coordinate _coord60 = Coordinate.From(6, 0);
    readonly Coordinate _coord61 = Coordinate.From(6, 1);
    readonly Coordinate _coord62 = Coordinate.From(6, 2);
    readonly Coordinate _coord63 = Coordinate.From(6, 3);
    readonly Coordinate _coord64 = Coordinate.From(6, 4);
    readonly Coordinate _coord65 = Coordinate.From(6, 5);
    readonly Coordinate _coord66 = Coordinate.From(6, 6);
    private static List<Coordinate> Sort(List<Coordinate> coordinates) => coordinates.OrderBy(c => c).ToList();
    #endregion

    [Fact]
    public void Return0_0WhenBoardIsEmpty()
    {
        var board = Board.Empty;
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        result.ShouldBe(new List<Coordinate> { new(0, 0) });
    }

    [Fact]
    public void ReturnAroundWhenSingleTileOnBoard()
    {
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var coord1 = Coordinate.From(-1, 0);
        var coord3 = Coordinate.From(0, -1);
        var coord4 = Coordinate.From(0, 0);
        var coord5 = Coordinate.From(0, 1);
        var coord7 = Coordinate.From(1, 0);
        var singleTile = new List<TileOnBoard> { new(tile, coord4) };
        var board = Board.From(singleTile);
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        var expected = new List<Coordinate> { coord1, coord3, coord5, coord7 };
        Sort(result).ShouldBe(Sort(expected));
    }

    [Fact]
    public void ReturnAroundWhen2TilesOnBoard()
    {
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var tiles = new List<TileOnBoard> { new(tile, _coord11), new(tile, _coord12) };
        var board = Board.From(tiles);
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        var expected = new List<Coordinate> { _coord01, _coord02, _coord10, _coord13, _coord21, _coord22 };
        Sort(result).ShouldBe(Sort(expected));
    }

    [Fact]
    public void ReturnAroundWhenLotOfTilesOnBoard()
    {
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var tiles = new List<TileOnBoard>
        {
            new(tile, _coord31), new(tile, _coord41),
            new(tile, _coord42),
            new(tile, _coord13), new(tile, _coord23), new(tile, _coord33),new(tile, _coord43),
            new(tile, _coord34), new(tile, _coord54),
            new(tile, _coord35), new(tile, _coord45), new(tile, _coord55),
        };
        var board = Board.From(tiles);
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        var expected = new List<Coordinate>
        {
            _coord30, _coord40,
            _coord21, _coord51,
            _coord12, _coord22, _coord32, _coord52,
            _coord03, _coord53,
            _coord14, _coord24, _coord44, _coord64,
            _coord25, _coord65,
            _coord36, _coord46, _coord56,
        };
        Sort(result).ShouldBe(Sort(expected));
    }
}