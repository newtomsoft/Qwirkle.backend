namespace Qwirkle.UltraBoardGames.Player.WebDriverFactory;

public class OperaDriverFactory : IWebDriverFactory
{
    public IWebDriver CreateDriver()
    {
        var rootDirectory = Directory.GetCurrentDirectory();
        var extensionsDirectory = Path.Combine(rootDirectory, "Resources", "Opera", "Extensions");

        var options = new OperaOptions();

        new DriverManager().SetUpDriver(new OperaConfig());
        var driver = new OperaDriver();
        return driver;
    }
}