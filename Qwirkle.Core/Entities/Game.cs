namespace Qwirkle.Core.Entities;
public class Game
{
    public int Id { get; }
    public Board Board { get; }
    public List<Player> Players { get; set; }
    public Bag Bag { get; set; }
    public bool GameOver { get; set; }

    public Game(int id, Board board, List<Player> players, bool gameOver, Bag bag = null) //todo board
    {
        Id = id;
        Board = board;
        Players = players;
        Bag = bag;
        GameOver = gameOver;
    }
}
