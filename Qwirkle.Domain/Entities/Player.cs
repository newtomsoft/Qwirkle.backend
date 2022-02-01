namespace Qwirkle.Domain.Entities;

public record Player
{
    public int Id { get; }
    public int UserId { get; }
    public int GameId { get; }
    public string Pseudo { get; }
    public int GamePosition { get; set; }
    public int Points { get; set; }
    public int LastTurnPoints { get; set; }
    public Rack Rack { get; set; }
    public bool IsTurn { get; private set; }
    public bool LastTurnSkipped { get; set; }
    public User User { get; }

    public Player(int id, int userId, int gameId, string pseudo, int gamePosition, int points, int lastTurnPoints, Rack rack, bool isTurn, bool lastTurnSkipped, User user = default)
    {
        Id = id;
        UserId = userId;
        GameId = gameId;
        Pseudo = pseudo;
        GamePosition = gamePosition;
        Points = points;
        LastTurnPoints = lastTurnPoints;
        Rack = rack;
        IsTurn = isTurn;
        LastTurnSkipped = lastTurnSkipped;
        User = user;
    }

    public Player(Player player)
    {
        Id = player.Id;
        UserId = player.UserId;
        GameId = player.GameId;
        Pseudo = player.Pseudo;
        GamePosition = player.GamePosition;
        Points = player.Points;
        LastTurnPoints = player.LastTurnPoints;
        Rack = new Rack(player.Rack.Tiles.ConvertAll(x => x));
        IsTurn = player.IsTurn;
        LastTurnSkipped = player.LastTurnSkipped;
        User = player.User;
    }
    public void SetTurn(bool turn) => IsTurn = turn;

    public bool HasTiles(IEnumerable<Tile> tiles) => tiles.All(tile => Rack.Tiles.Select(t => (t.Color, t.Shape)).Contains((tile.Color, tile.Shape)));

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
                if (tiles[i].Color == tiles[j].Color && tiles[i].Shape != tiles[j].Shape) sameColor++;
                if (tiles[i].Color != tiles[j].Color && tiles[i].Shape == tiles[j].Shape) sameShape++;
            }
            maxSameColor = Math.Max(maxSameColor, sameColor);
            maxSameShape = Math.Max(maxSameShape, sameShape);
        }
        return Math.Max(maxSameColor, maxSameShape) + 1;
    }

    public Player GetWithoutTiles() => this with { Rack = new Rack(new List<TileOnPlayer>()) };
}