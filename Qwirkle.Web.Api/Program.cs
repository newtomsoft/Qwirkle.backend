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
            var host = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                { 
                    config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json"), optional: true); //When start with debugging
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true); //When start without debugging
                })
                .UseStartup<Startup>();
            return host;
        }
    }
}
