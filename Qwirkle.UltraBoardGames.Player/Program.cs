var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOptions();
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<BotUseCase>();
        services.AddSingleton<CoreUseCase>();
        services.AddSingleton<InfoUseCase>();
        services.AddSingleton<UltraBoardGamesPlayerApplication>();
        services.AddSingleton<IArtificialIntelligence, ArtificialIntelligence>();
        services.AddSingleton<INotification, FakeNotification>();
        services.AddSingleton<IRepository, FakeRepository>();
        services.AddDbContext<DefaultDbContext>();
    })
    .UseConsoleLifetime()
    .Build();

using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var serviceProvider = services.GetRequiredService<UltraBoardGamesPlayerApplication>();
serviceProvider.Run();

