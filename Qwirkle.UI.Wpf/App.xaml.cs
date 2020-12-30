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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
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

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName)
            .AddJsonFile(@"..\sharesettings.Development.json", optional: true) //When dev
            .AddJsonFile("sharesettings.Development.json", optional: true) //When published
            .AddJsonFile(@"D:\Boulot\projets info\Qwirkle\sharesettings.Development.json", optional: true); //TODO!!!
            IConfigurationRoot configuration = builder.Build();

            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, configuration);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            MainWindow mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
