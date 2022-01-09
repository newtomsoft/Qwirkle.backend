namespace Qwirkle.UltraBoardGames.Player.WebDriverFactory;

public class FirefoxDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var profile = new FirefoxProfile();
        profile.AddExtension("WebPlugin/ublock_origin-1.39.2-an+fx.xpi");
        var options = new FirefoxOptions { Profile = profile };
        var driver = new FirefoxDriver(options);
        return driver;
    }
}
