namespace Qwirkle.Infra.Repository;

public class DefaultDbContext : IdentityDbContext<UserDao, IdentityRole<int>, int>
{
    public DbSet<TileDao> Tiles { get; set; }
    public DbSet<TileOnBagDao> TilesOnBag { get; set; }
    public DbSet<TileOnBoardDao> TilesOnBoard { get; set; }
    public DbSet<TileOnPlayerDao> TilesOnPlayer { get; set; }
    public DbSet<GameDao> Games { get; set; }
    public DbSet<PlayerDao> Players { get; set; }
    public override DbSet<UserDao> Users { get; set; }


    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
        // Method intentionally left empty.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Method intentionally left empty.
    }

    private const string RoleBotName = "Bot";
    private const int RoleBotId = 1;
    private const string RoleAdminName = "Admin";
    private const int RoleAdminId = 2;
    private const string RoleGuestName = "Guest";
    private const int RoleGuestId = 3;
    private const string RoleUserName = "User";
    private const int RoleUserId = 4;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        CreateTiles(builder);
        CreateBots(builder, 1, 4);
        CreateAdmins(builder, 5);
        CreateGuestRole(builder);
        CreateUserRole(builder);
    }

    private static void CreateTiles(ModelBuilder builder)
    {
        const int numberOfSameTile = 3;
        var tileId = 1;
        for (var i = 0; i < numberOfSameTile; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    builder.Entity<TileDao>().HasData(new TileDao { Id = tileId++, Shape = shape, Color = color });
    }

    private static void CreateBots(ModelBuilder builder, int idStart, int botsNumber)
    {
        builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int> { Id = RoleBotId, Name = RoleBotName, NormalizedName = RoleBotName.ToUpper() });
        for (var userId = idStart; userId < idStart + botsNumber; userId++)
        {
            var botName = "bot" + userId;
            var botEmail = botName + "@bot";
            builder.Entity<UserDao>().HasData(new UserDao { Id = userId, UserName = botName, NormalizedUserName = botName.ToUpper(), Email = botEmail });
            builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int> { UserId = userId, RoleId = RoleBotId });
        }
    }

    private static void CreateAdmins(ModelBuilder builder, int userIdStart)
    {
        const string tomPassword0 = "AQAAAAEAACcQAAAAED29";
        const string tomPassword1 = "kKSVgjTdA6s6pXQ0a+7iy9MJ5Y1byxFl2MWZn";
        const string tomPassword2 = "X4WE6lw1SsR9FGeGypraM3G+g==";
        const string jcPassword0 = "AQAAAAEAACcQAAAAE";
        const string jcPassword1 = "JOr0iSf9bL59UJqwWyCpcjdampHsvulqOZ/";
        const string jcPassword2 = "NTApuuwLJsc1Sf9xRquQWPIz2S8rUQ==";

        var userId = userIdStart;
        builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int> { Id = RoleAdminId, Name = RoleAdminName, NormalizedName = RoleAdminName.ToUpper() });

        builder.Entity<UserDao>().HasData(new UserDao { Id = userId, UserName = "Tom", NormalizedUserName = "TOM", Email = "thomas@newtomsoft.fr", NormalizedEmail = "THOMAS@NEWTOMSOFT.FR", FirstName = "Thomas", LastName = "Vuille", PasswordHash = tomPassword0 + tomPassword1 + tomPassword2, LockoutEnabled = false, SecurityStamp = Guid.NewGuid().ToString("N").ToUpper() });
        builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int> { UserId = userId, RoleId = RoleAdminId });
        userId++;
        builder.Entity<UserDao>().HasData(new UserDao { Id = userId, UserName = "JC", NormalizedUserName = "JC", Email = "jc@jc.fr", NormalizedEmail = "JC@JC.FR", FirstName = "Jean Charles", LastName = "Gouleau", PasswordHash = jcPassword0 + jcPassword1 + jcPassword2, LockoutEnabled = false, SecurityStamp = Guid.NewGuid().ToString("N").ToUpper() });
        builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int> { UserId = userId, RoleId = RoleAdminId });
    }

    private static void CreateGuestRole(ModelBuilder builder) => builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int> { Id = RoleGuestId, Name = RoleGuestName, NormalizedName = RoleGuestName.ToUpper() });

    private static void CreateUserRole(ModelBuilder builder) => builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int> { Id = RoleUserId, Name = RoleUserName, NormalizedName = RoleUserName.ToUpper() });
}
