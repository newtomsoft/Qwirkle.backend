namespace Qwirkle.UltraBoardGames.Player.WebDriverFactory;

public class EdgeDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var options = new EdgeOptions();
        options.AddExtensions("WebPlugin/uBlock-Origin.edge.crx");
        //options.AddArgument("--headless");
        var driver = new EdgeDriver(options);
        return driver;
    }
}
