namespace Qwirkle.Core.Entities;

public class User
{
    public string Pseudo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Help { get; set; }
    public int Points { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public List<Player> Players { get; set; }
}