using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.Tools;
using Qwirkle.Infra.Repository;

namespace Data
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args) => EntityFrameworkTools<DefaultDbContext>.CreateDbContext();
    }
}