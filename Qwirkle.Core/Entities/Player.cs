namespace Qwirkle.Core.Entities;
public class Player
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Pseudo { get; set; }
    public int GameId { get; set; }
    public int GamePosition { get; set; }
    public int Points { get; set; }
    public int LastTurnPoints { get; set; }
    public Rack Rack { get; set; }
    public bool LastTurnSkipped { get; set; }
    public bool IsTurn { get; private set; }

    public Player(int id, int userId, int gameId, string pseudo, int gamePosition, int points, int lastTurnPoints, List<TileOnPlayer> tiles, bool isTurn, bool lastTurnSkipped) // todo remplacer tiles par rack
    {
        Id = id;
        UserId = userId;
        GameId = gameId;
        Pseudo = pseudo;
        GamePosition = gamePosition;
        Points = points;
        LastTurnPoints = lastTurnPoints;
        Rack = new Rack(tiles);
        IsTurn = isTurn;
        LastTurnSkipped = lastTurnSkipped;
    }


    public void SetTurn(bool turn) => IsTurn = turn;
    public bool HasTiles(IEnumerable<int> tilesIds) => tilesIds.All(id => Rack.Tiles.Select(t => t.Id).Contains(id));
    public int TilesNumberCanBePlayedAtGameBeginning()
    {
        var tiles = Rack.Tiles;
        var maxSameColor = 0;
        var maxSameShape = 0;
        for (var i = 0; i < tiles.Count; i++)
        {
            var sameColor = 0;
            var sameShape = 0;
            for (var j = i + 1; j < tiles.Count; j++)
            {
                if (tiles[i].Color == tiles[j].Color && tiles[i].Shape != tiles[j].Shape)
                    sameColor++;
                if (tiles[i].Color != tiles[j].Color && tiles[i].Shape == tiles[j].Shape)
                    sameShape++;
            }
            maxSameColor = Math.Max(maxSameColor, sameColor);
            maxSameShape = Math.Max(maxSameShape, sameShape);
        }
        return Math.Max(maxSameColor, maxSameShape) + 1;
    }
}
