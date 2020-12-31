using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.Tools;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.GameContext.Services;
using Qwirkle.Core.PlayerContext.Ports;
using Qwirkle.Core.PlayerContext.Services;
using Qwirkle.Infra.Persistence;
using Qwirkle.Infra.Persistence.Adapters;
using Qwirkle.UI.Wpf.Views;
using System;
using System.Configuration;
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
            services.AddScoped<IGamePersistence, GamePersistenceAdapter>();
            services.AddScoped<IPlayerPersistence, PlayerPersistenceAdapter>();

            services.AddScoped<IRequestCompliance, ComplianceService>();
            services.AddScoped<IRequestGame, GameService>();
            services.AddScoped<IRequestPlayer, PlayerService>();

            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, GetConfiguration());

            var serviceProvider = services.BuildServiceProvider();
            var mainWindow = serviceProvider.GetService<MainWindow>();
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
