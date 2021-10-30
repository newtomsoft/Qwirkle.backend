namespace Qwirkle.Core.Entities;
public class Game
{
    public int Id { get; }
    public Board Board { get; }
    public List<Player> Players { get; set; }
    public Bag Bag { get; set; }
    public bool GameOver { get; set; }

    public Game(int id, List<TileOnBoard> tiles, List<Player> players, bool gameOver, Bag bag = null) //todo board
    {
        Id = id;
        Board = new Board(tiles);
        Players = players;
        Bag = bag;
        GameOver = gameOver;
    }

    public Game(Game game)
    {
        Id = game.Id;
        Board = game.Board;
        Players = game.Players;
        Bag = game.Bag;
        GameOver = game.GameOver;
    }
}
