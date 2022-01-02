namespace Qwirkle.Infra.Repository;

public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
{
    public DefaultDbContext CreateDbContext(string[] args)
    {
        // use command dotnet ef Database update --connection "<connectionString>"
        // exemple :   dotnet ef Database update --connection "Server=localhost;Database=qwirkle.dev;Trusted_Connection=True;"
        var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
        optionsBuilder.UseSqlServer("replaced by option --connection");
        return new DefaultDbContext(optionsBuilder.Options);
    }
}
