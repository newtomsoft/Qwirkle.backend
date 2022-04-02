namespace Qwirkle.Domain.Entities;

public class User
{
    public string Pseudo { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public int Help { get; }
    public int Points { get; }
    public int GamesPlayed { get; }
    public int GamesWon { get; }
    public List<Player> Players { get; }
    public HashSet<User> Friends { get; }

    public User() { } //TODO

    public User(string pseudo, string email, string firstName = default, string lastName = default, int help = 0, int points = 0, int gamesPlayed = 0, int gamesWon = 0, Player player = default, HashSet<User> friends = default)
    {
        Pseudo = pseudo;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Help = help;
        Points = points;
        GamesPlayed = gamesPlayed;
        GamesWon = gamesWon;
        Friends = friends;
        Players = new List<Player> { player };
    }

    public void AddFriend(User friend) => Friends.Add(friend);
}