namespace Qwirkle.Domain.Entities;

public class User
{
    public string Pseudo { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Help { get; set; }
    public int Points { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public List<Player> Players { get; set; }


    public User(string pseudo, string email, string firstName = default, string lastName = default, int help = 0, int points = 0, int gamesPlayed = 0, int gamesWon = 0, Player player = default)
    {
        Pseudo = pseudo;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Help = help;
        Points = points;
        GamesPlayed = gamesPlayed;
        GamesWon = gamesWon;
        Players = new List<Player> { player };
    }
}