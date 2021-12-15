namespace Qwirkle.Domain.Entities;

public class Game
{
    public int Id { get; }
    public Board Board { get; }
    public List<Player> Players { get; set; }
    public Bag Bag { get; set; }
    public bool GameOver { get; set; }

    public Game(int id, Board board, List<Player> players, bool gameOver, Bag bag = null)
    {
        Id = id;
        Board = board;
        Players = players;
        Bag = bag;
        GameOver = gameOver;
    }
    public Game(Game game)
    {
        Id = game.Id;
        Board = new Board(game.Board.Tiles.ConvertAll(x => x));
        Players = game.Players.ConvertAll(x => new Player(x));
        Bag = new Bag(game.Id, game.Bag.Tiles.ConvertAll(x => x));
        GameOver = game.GameOver;
    }
}