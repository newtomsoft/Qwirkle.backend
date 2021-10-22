using Qwirkle.Infra.Repository;
using Xunit;

namespace Qwirkle.Core.ComplianceRepository.Tests
{
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
}
