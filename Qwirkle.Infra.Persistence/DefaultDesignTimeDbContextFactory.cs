using Microsoft.EntityFrameworkCore.Design;
using Qwirkle.Infra.Persistence;
using Newtomsoft.Tools;

namespace Data
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args) => EntityFrameworkTools<DefaultDbContext>.CreateDbContext("Qwirkle.Web.Api");
    }
}