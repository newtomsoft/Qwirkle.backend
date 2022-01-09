var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddOptions();
        services.AddSingleton<UltraBoardGamesPlayerApplication>();
        services.AddSingleton<IWebDriverFactory, EdgeDriverFactory>();
        services.AddSingleton<BotService>();
        services.AddSingleton<CoreService>();
        services.AddSingleton<InfoService>();
        services.AddSingleton<GameScraper>();
        services.AddSingleton<INotification, NoNotification>();
        services.AddSingleton<IRepository, NoRepository>();
        services.AddDbContext<DefaultDbContext>();
    })
    .ConfigureLogging((_, builder) =>
    {
        builder.AddFile("Logs/QwirkleUltraBoardGames-{Date}.txt");
    })
    .UseConsoleLifetime()
    .Build();

using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var application = services.GetRequiredService<UltraBoardGamesPlayerApplication>();


application.Run();

