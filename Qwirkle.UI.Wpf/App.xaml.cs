using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.Tools;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Infra.Persistence;
using Qwirkle.Infra.Persistence.Adapters;
using Qwirkle.UI.Wpf.Views;
using System;
using System.Windows;
using System.IO;

namespace Qwirkle.UI.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs startupEventArgs)
        {
            base.OnStartup(startupEventArgs);

            ServiceCollection services = new ServiceCollection();
            services.AddScoped<MainWindow>();
            services.AddScoped<ICompliancePersistence, CompliancePersistenceAdapter>();
            services.AddScoped<IRequestCompliance, ComplianceService>();

            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, GetConfiguration());

            var mainWindow = services.BuildServiceProvider().GetService<MainWindow>();
            mainWindow.Show();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName) //todo not work!!!
                        .AddJsonFile(@"..\sharesettings.Development.json", optional: true) //When dev //TODO not work!!!
                        .AddJsonFile("sharesettings.Development.json", optional: true) //When published //TODO not work!!!
                        .AddJsonFile(@"D:\Boulot\projets info\Qwirkle\sharesettings.Development.json", optional: true); //TODO!!!
            var configuration = builder.Build();
            return configuration;
        }
    }
}
