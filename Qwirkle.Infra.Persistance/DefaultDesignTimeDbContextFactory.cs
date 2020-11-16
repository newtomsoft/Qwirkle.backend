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
            DbContextOptionsBuilder<DefaultDbContext> optionBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
            string path = Directory.GetCurrentDirectory();
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            string persistance = Environment.GetEnvironmentVariable("PERSISTANCE");
            string dataBase;
            if (persistance == "SqlServer")
                dataBase = "AdminDbContext";
            else
                dataBase = "Sqlite";

            Console.WriteLine($"ASPNETCORE_ENVIRONMENT is : {env}");
            Console.WriteLine($"PERSISTANCE is : {persistance} ; data Base is : {dataBase}");

            IConfigurationBuilder builder = new ConfigurationBuilder()
                               .SetBasePath(path)
                               .AddJsonFile($"appsettings.{env}.json");
            IConfigurationRoot config = builder.Build();
            string connectionString = config.GetConnectionString(dataBase);
            if (persistance == "SqlServer")
                optionBuilder.UseSqlServer(connectionString);
            else
                optionBuilder.UseSqlite(connectionString);
            return new DefaultDbContext(optionBuilder.Options);
        }
    }
}