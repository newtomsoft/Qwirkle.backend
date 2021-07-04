using Microsoft.EntityFrameworkCore.Design;
using Newtomsoft.EntityFramework.Core;

namespace Qwirkle.Infra.Repository
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args) => EntityFrameworkTools<DefaultDbContext>.CreateDbContext();
    }
}