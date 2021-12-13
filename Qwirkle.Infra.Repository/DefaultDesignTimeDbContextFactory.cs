namespace Qwirkle.Infra.Repository;

public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
{
    public DefaultDbContext CreateDbContext(string[] args)
    {
#warning todo connectionStrings
        const string connectionString = "";
        var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new DefaultDbContext(optionsBuilder.Options);
    }
}
