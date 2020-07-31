using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Persistance;
using System;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class ConnectionFactory : IDisposable
    {

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public DefaultDbContext CreateContextForInMemory()
        {
            var option = new DbContextOptionsBuilder<DefaultDbContext>().UseInMemoryDatabase(databaseName: "Test_Database").Options;

            var context = new DefaultDbContext(option);
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

        public DefaultDbContext CreateContextForSQLite()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var option = new DbContextOptionsBuilder<DefaultDbContext>().UseSqlite(connection).Options;

            var context = new DefaultDbContext(option);
            
            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
