using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Qwirkle.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var host= WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", $"sharesettings.{hostContext.HostingEnvironment.EnvironmentName}.json"), optional: true); //When dev
                    config.AddJsonFile($"sharesettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true); //When published
                })
                .UseStartup<Startup>();
            return host;
        }
    }
}
