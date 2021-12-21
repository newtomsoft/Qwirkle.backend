namespace Qwirkle.Domain.Entities;

public record Game(int Id, Board Board, List<Player> Players, Bag Bag, bool GameOver)
{
    public Game(int id, Board board, List<Player> players, bool gameOver) : this(id, board, players, Bag.Empty(), gameOver)
    { }

    public Game(Board board, List<Player> players) : this(0, board, players, Bag.Empty(), false)
    { }

    public Game() : this(0, Board.Empty(), new List<Player>(), Bag.Empty(), false)
    { }

    public Game(Game game)
    {
        Id = game.Id;
<<<<<<< HEAD
        Board = new Board(game.Board.Tiles.ConvertAll(x => x));
        Players = game.Players.ConvertAll(x => new Player(x));
        Bag = new Bag(game.Id, game.Bag.Tiles.ConvertAll(x => x));
=======
        Board = new Board(game.Board.Tiles);
        Players = game.Players.Select(x => new Player(x)).ToList();
        Bag = new Bag(game.Id, game.Bag.Tiles.Select(x => x).ToList());
>>>>>>> 313cc98a871d96738191983678b46b4253babc2d
        GameOver = game.GameOver;
    }
}