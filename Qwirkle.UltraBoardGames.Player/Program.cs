﻿using System.Runtime.InteropServices;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.WriteLine("Windows");
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    Console.WriteLine("Linux");
}

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddOptions();
        services.AddSingleton<UltraBoardGamesPlayerApplication>();
        services.AddSingleton<IWebDriverFactory, EdgeDriverFactory>();
        services.AddSingleton<IAuthentication, NoAuthentication>();
        services.AddSingleton<GameScraper>();
        services.AddSingleton<BotService>();
        services.AddSingleton<CoreService>();
        services.AddSingleton<UserService>();
        services.AddSingleton<InfoService>();
        services.AddSingleton<INotification, NoNotification>();
        services.AddSingleton<IRepository, NoRepository>();
        services.AddDbContext<NoDbContext>();
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