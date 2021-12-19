namespace Qwirkle.UltraBoardGames.Player;

public class EdgeDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var options = new EdgeOptions();
        options.AddExtensions("uBlock-Origin.edge.crx");
        var driver = new EdgeDriver(options);
        return driver;
    }
}
