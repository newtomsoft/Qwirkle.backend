namespace Qwirkle.UltraBoardGames.Player.WebDriverFactory;

public class FirefoxDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var rootDirectory = Directory.GetCurrentDirectory();
        var profileDirectory = Path.Combine(rootDirectory, "Resources", "Firefox", "Profile");
        var profile = new FirefoxProfile(profileDirectory);
        var options = new FirefoxOptions { Profile = profile };
        new DriverManager().SetUpDriver(new FirefoxConfig());
        var driver = new FirefoxDriver(options);
        return driver;
    }
}