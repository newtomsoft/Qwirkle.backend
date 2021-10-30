namespace ComplianceRepository.Test;

public class DefaultDesignTimeDbContextFactoryShould
{
    #region private attributs

    #endregion



    #region private methods

    #endregion

    [Fact]
    public void Prout()
    {
        DefaultDesignTimeDbContextFactory designDbContextFactory = new();
        designDbContextFactory.CreateDbContext(null);
    }
}
