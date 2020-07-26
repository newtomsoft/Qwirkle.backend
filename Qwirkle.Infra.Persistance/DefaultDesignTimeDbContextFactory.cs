using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Qwirkle.Infra.Persistance;
using System;
using System.IO;

namespace Data
{
    public class DefaultDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
    {
        public DefaultDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine("CreateDbContext");
            string path = Directory.GetCurrentDirectory();
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == null)
                env = "Development";
            Console.WriteLine("ASPNETCORE_ENVIRONMENT is : " + env);
            IConfigurationBuilder builder = new ConfigurationBuilder()
                               .SetBasePath(path)
                               .AddJsonFile($"appsettings.{env}.json");
            IConfigurationRoot config = builder.Build();
            string connectionString = config.GetConnectionString("AdminDbContext");
            DbContextOptionsBuilder<DefaultDbContext> optionBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
            optionBuilder.UseSqlServer(connectionString);
            return new DefaultDbContext(optionBuilder.Options);
        }
    }
}