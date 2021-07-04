using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtomsoft.EntityFramework.Core;
using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.UI.Wpf.ViewModels;
using System.Windows;

namespace Qwirkle.UI.Wpf
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddScoped<MainWindow>();
            services.AddSingleton<IConfiguration>(GetConfiguration());
            services.AddSingleton(GetLogger());
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<ICoreUseCase, CoreUseCase>();
            var configuration = GetConfiguration();
            EntityFrameworkTools<DefaultDbContext>.AddDbContext(services, configuration);
            services.AddSingleton<MainViewModel>();
        }

        private static ILogger GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());
            return loggerFactory.CreateLogger<App>();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                        .AddJsonFile("sharesettings.Development.json", optional: true);

            return builder.Build();
        }
    }
}
