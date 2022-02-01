// ReSharper disable UnusedMember.Local
namespace Qwirkle.Test;

public class GetAdjoiningCoordinatesToTilesShould
{
    #region private
    readonly Coordinates _coord00 = Coordinates.From(0, 0);
    readonly Coordinates _coord01 = Coordinates.From(0, 1);
    readonly Coordinates _coord02 = Coordinates.From(0, 2);
    readonly Coordinates _coord03 = Coordinates.From(0, 3);
    readonly Coordinates _coord04 = Coordinates.From(0, 4);
    readonly Coordinates _coord05 = Coordinates.From(0, 5);
    readonly Coordinates _coord06 = Coordinates.From(0, 6);
    readonly Coordinates _coord10 = Coordinates.From(1, 0);
    readonly Coordinates _coord11 = Coordinates.From(1, 1);
    readonly Coordinates _coord12 = Coordinates.From(1, 2);
    readonly Coordinates _coord13 = Coordinates.From(1, 3);
    readonly Coordinates _coord14 = Coordinates.From(1, 4);
    readonly Coordinates _coord15 = Coordinates.From(1, 5);
    readonly Coordinates _coord16 = Coordinates.From(1, 6);
    readonly Coordinates _coord20 = Coordinates.From(2, 0);
    readonly Coordinates _coord21 = Coordinates.From(2, 1);
    readonly Coordinates _coord22 = Coordinates.From(2, 2);
    readonly Coordinates _coord23 = Coordinates.From(2, 3);
    readonly Coordinates _coord24 = Coordinates.From(2, 4);
    readonly Coordinates _coord25 = Coordinates.From(2, 5);
    readonly Coordinates _coord26 = Coordinates.From(2, 6);
    readonly Coordinates _coord30 = Coordinates.From(3, 0);
    readonly Coordinates _coord31 = Coordinates.From(3, 1);
    readonly Coordinates _coord32 = Coordinates.From(3, 2);
    readonly Coordinates _coord33 = Coordinates.From(3, 3);
    readonly Coordinates _coord34 = Coordinates.From(3, 4);
    readonly Coordinates _coord35 = Coordinates.From(3, 5);
    readonly Coordinates _coord36 = Coordinates.From(3, 6);
    readonly Coordinates _coord40 = Coordinates.From(4, 0);
    readonly Coordinates _coord41 = Coordinates.From(4, 1);
    readonly Coordinates _coord42 = Coordinates.From(4, 2);
    readonly Coordinates _coord43 = Coordinates.From(4, 3);
    readonly Coordinates _coord44 = Coordinates.From(4, 4);
    readonly Coordinates _coord45 = Coordinates.From(4, 5);
    readonly Coordinates _coord46 = Coordinates.From(4, 6);
    readonly Coordinates _coord50 = Coordinates.From(5, 0);
    readonly Coordinates _coord51 = Coordinates.From(5, 1);
    readonly Coordinates _coord52 = Coordinates.From(5, 2);
    readonly Coordinates _coord53 = Coordinates.From(5, 3);
    readonly Coordinates _coord54 = Coordinates.From(5, 4);
    readonly Coordinates _coord55 = Coordinates.From(5, 5);
    readonly Coordinates _coord56 = Coordinates.From(5, 6);
    readonly Coordinates _coord60 = Coordinates.From(6, 0);
    readonly Coordinates _coord61 = Coordinates.From(6, 1);
    readonly Coordinates _coord62 = Coordinates.From(6, 2);
    readonly Coordinates _coord63 = Coordinates.From(6, 3);
    readonly Coordinates _coord64 = Coordinates.From(6, 4);
    readonly Coordinates _coord65 = Coordinates.From(6, 5);
    readonly Coordinates _coord66 = Coordinates.From(6, 6);
    private static List<Coordinates> Sort(List<Coordinates> coordinates) => coordinates.OrderBy(c => c).ToList();
    #endregion

    [Fact]
    public void Return0_0WhenBoardIsEmpty()
    {
        var board = Board.Empty();
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        result.ShouldBe(new List<Coordinates> { new(0, 0) });
    }

    [Fact]
    public void ReturnAroundWhenSingleTileOnBoard()
    {
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var coord1 = Coordinates.From(-1, 0);
        var coord3 = Coordinates.From(0, -1);
        var coord4 = Coordinates.From(0, 0);
        var coord5 = Coordinates.From(0, 1);
        var coord7 = Coordinates.From(1, 0);
        var singleTile = new List<TileOnBoard> { new(tile, coord4) };
        var board = Board.From(singleTile);
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        var expected = new List<Coordinates> { coord1, coord3, coord5, coord7 };
        Sort(result).ShouldBe(Sort(expected));
    }

    [Fact]
    public void ReturnAroundWhen2TilesOnBoard()
    {
        var tile = new Tile(TileColor.Blue, TileShape.Circle);
        var tiles = new List<TileOnBoard> { new(tile, _coord11), new(tile, _coord12) };
        var board = Board.From(tiles);
        var result = board.GetFreeAdjoiningCoordinatesToTiles();
        var expected = new List<Coordinates> { _coord01, _coord02, _coord10, _coord13, _coord21, _coord22 };
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
        var expected = new List<Coordinates>
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