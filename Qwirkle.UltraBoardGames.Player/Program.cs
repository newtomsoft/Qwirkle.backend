var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddOptions();
        services.AddSingleton<UltraBoardGamesPlayerApplication>();
        services.AddSingleton<BotUseCase>();
        services.AddSingleton<CoreUseCase>();
        services.AddSingleton<InfoUseCase>();
        services.AddSingleton<GameScraper>();
        services.AddSingleton<INotification, FakeNotification>();
        services.AddSingleton<IArtificialIntelligence, ArtificialIntelligence>();
        services.AddSingleton<IRepository, FakeRepository>();
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

