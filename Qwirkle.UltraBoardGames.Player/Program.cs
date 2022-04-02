var hostBuilder = Host.CreateDefaultBuilder(args);
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

hostBuilder.UseSerilog((_, config) => config.ReadFrom.Configuration(configuration));
var host = hostBuilder
    .ConfigureServices((_, services) =>
    {
        services.AddOptions();
        services.AddSingleton<UltraBoardGamesPlayerApplication>();
        services.AddSingleton<IWebDriverFactory, ChromeDriverFactory>();
        services.AddSingleton<IAuthentication, NoAuthentication>();
        services.AddSingleton<BotService>();
        services.AddSingleton<CoreService>();
        services.AddSingleton<UserService>();
        services.AddSingleton<InfoService>();
        services.AddSingleton<INotification, NoNotification>();
        services.AddSingleton<IRepository, NoRepository>();
        services.AddDbContext<NoDbContext>();
    })
    .UseConsoleLifetime()
    .Build();

using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var application = services.GetRequiredService<UltraBoardGamesPlayerApplication>();

application.Run(RequestGamesNumber());

static int RequestGamesNumber()
{
    int gamesNumber;
    while (true)
    {
        Console.WriteLine("Number of games to play (1 - 20) ?");
        var userInput = Console.ReadLine();
        var isNumber = int.TryParse(userInput, out gamesNumber);
        if (isNumber && gamesNumber is >= 1 and <= 20) break;
    }
    return gamesNumber;
}