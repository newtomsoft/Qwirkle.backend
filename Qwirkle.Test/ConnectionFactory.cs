namespace Qwirkle.Test;

public sealed class ConnectionFactory : IDisposable
{

    #region IDisposable Support
    private bool _disposedValue; // To detect redundant calls

    public static DefaultDbContext CreateContextForInMemory()
    {
        var option = new DbContextOptionsBuilder<DefaultDbContext>().UseInMemoryDatabase(databaseName: "Test_Database").Options;
        var context = new DefaultDbContext(option);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    public DefaultDbContext CreateContextForSqLite()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var option = new DbContextOptionsBuilder<DefaultDbContext>().UseSqlite(connection).Options;
        var context = new DefaultDbContext(option);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }


    private void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
        }
        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
    }
    #endregion
}
