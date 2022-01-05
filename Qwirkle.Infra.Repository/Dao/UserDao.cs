﻿namespace Qwirkle.Infra.Repository.Dao;

[Table("User")]
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
}
