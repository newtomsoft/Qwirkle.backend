namespace Qwirkle.UltraBoardGames.Player.WebDriverFactory;

public class ChromeDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var extensionsDirectory = Path.Combine("Resources", "Chrome", "uBlock-Origin.crx");
        var options = new ChromeOptions();
        options.AddExtension(extensionsDirectory);
        options.AddExcludedArgument("enable-automation");
        new DriverManager().SetUpDriver(new ChromeConfig());
        var driver = new ChromeDriver(options);
        return driver;
    }
}