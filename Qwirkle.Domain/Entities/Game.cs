namespace Qwirkle.Domain.Entities;

public record Game(int Id, Board Board, List<Player> Players, Bag Bag, bool GameOver)
{
    public Game(int id, Board board, List<Player> players, bool gameOver) : this(id, board, players, Bag.Empty(), gameOver)
    { }
    public Game(int id, Board board, List<Player> players) : this(id, board, players, Bag.Empty(), true)
    { }
    public Game(Board board, List<Player> players) : this(0, board, players, Bag.Empty(), false)
    { }

    public Game() : this(0, Board.Empty(), new List<Player>(), Bag.Empty(), false)
    { }

    public Game(Game game)
    {
        Id = game.Id;
        Board = new Board(game.Board.Tiles.ToHashSet());
        Players = game.Players.Select(x => new Player(x)).ToList();
        Bag = new Bag(game.Id, game.Bag.Tiles.Select(x => x).ToList());
        GameOver = game.GameOver;
    }

<<<<<<< HEAD
=======
    public bool IsBoardEmpty() => Board.Tiles.Count == 0;
    public bool IsBagEmpty() => Bag.Tiles.Count == 0;
>>>>>>> 9f45d2323e3f91f33c6b5d2197df2309e2d80464
}