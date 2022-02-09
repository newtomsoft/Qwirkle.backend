namespace Qwirkle.UltraBoardGames.Player.WebDriverFactory;

public class EdgeDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var options = new EdgeOptions();
        options.AddArgument("headless");
        options.AddArguments("--no-sandbox");
        new DriverManager().SetUpDriver(new EdgeConfig());
        var driver = new EdgeDriver(options);
        return driver;
    }
}
