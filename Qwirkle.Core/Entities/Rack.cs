namespace Qwirkle.Core.Entities;

public class Rack
{
    public List<TileOnPlayer> Tiles { get; }

    public Rack(List<TileOnPlayer> tiles)
    {
        Tiles = tiles;
    }

    public IEnumerable<Move> PossibleMoves(Board board)
    {

        //exemple
        var tile1 = Tiles[0].ToTileOnBoard(Coordinates.From(0, 0));
        var tile2 = Tiles[1].ToTileOnBoard(Coordinates.From(0, 1));
        var move1 = new Move(new List<TileOnBoard> { tile1, tile2 }, 2);
        var move2 = new Move(new List<TileOnBoard> { tile2, tile1 }, 2);
        return new List<Move> { move1, move2 };
    }
}