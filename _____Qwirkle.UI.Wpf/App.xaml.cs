using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtomsoft.Tools;
using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.UI.Wpf.Views;
using System.Windows;

namespace Qwirkle.UI.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs startupEventArgs)
        {
            base.OnStartup(startupEventArgs);
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<MainWindow>();
            services.AddScoped<IRepositoryPort, RepositoryAdapter>();
            services.AddScoped<ICommonUseCasePort, CommonUseCase>();
            var configuration = GetConfiguration();
            services.AddSingleton<IConfiguration>(GetConfiguration());
            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, configuration);
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
