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
                        .AddJsonFile("sharesettings.Development.json", optional: true);
            var configuration = builder.Build();
            return configuration;
        }
    }
}
