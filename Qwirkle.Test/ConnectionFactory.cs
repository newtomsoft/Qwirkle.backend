namespace Qwirkle.Test;

public sealed class ConnectionFactory : IDisposable
{

    #region IDisposable Support
    private bool _disposedValue; // To detect redundant calls
    private DefaultDbContext _dbContext = null!;

    private const int User1Id = 71;
    private const int User2Id = 21;
    private const int User3Id = 33;
    private const int User4Id = 14;

    public DefaultDbContext CreateContextForInMemory()
    {
        var option = new DbContextOptionsBuilder<DefaultDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _dbContext = new DefaultDbContext(option);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        return _dbContext;
    }

    public void Add4DefaultTestUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User1Id });
        _dbContext.Users.Add(new UserDao { Id = User2Id });
        _dbContext.Users.Add(new UserDao { Id = User3Id });
        _dbContext.Users.Add(new UserDao { Id = User4Id });
        _dbContext.SaveChanges();
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
