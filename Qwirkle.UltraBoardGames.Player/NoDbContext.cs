using Microsoft.EntityFrameworkCore;


namespace Qwirkle.UltraBoardGames.Player;

public class NoDbContext : DbContext
{
    public NoDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }
}
