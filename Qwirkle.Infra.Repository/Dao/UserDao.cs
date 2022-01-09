namespace Qwirkle.Infra.Repository.Dao;

[Table("User")]
[Index(nameof(UserName), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class UserDao : IdentityUser<int>
{
    [Column("Pseudo")]
    public override string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Help { get; set; }
    public int Points { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }

    public virtual List<UserDao> BookmarkedOpponents { get; set; } = new();
    public virtual List<UserDao> BookmarkedBy { get; set; }
}
