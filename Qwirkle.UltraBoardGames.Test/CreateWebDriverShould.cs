namespace Qwirkle.UltraBoardGames.Test;
public class CreateWebDriverShould
{
    public static TheoryData<IWebDriverFactory> WebDriverFactories => new()
    {
        new FirefoxDriverFactory(),
        new EdgeDriverFactory(),
        new ChromeDriverFactory(),
        //new OperaDriverFactory(),
    };

    [Theory]
    [MemberData(nameof(WebDriverFactories))]
    public void OpenWebBrowserAndCloseIt(IWebDriverFactory webDriverFactory)
    {
        IWebDriver? webDriver = null;
        var action = () => webDriver = webDriverFactory.CreateDriver();
        action.ShouldNotThrow();
        webDriver.ShouldNotBeNull();
        webDriver.Navigate().GoToUrl("https://www.google.com");
        webDriver.Title.Contains("Google").ShouldBeTrue();

        var count = webDriver.WindowHandles.Count;
        string handle = webDriver.CurrentWindowHandle;

        webDriver.Close();
        webDriver.Dispose();
    }

    [Fact]
    public void OtherTest()
    {
        var webDriverFactory = new ChromeDriverFactory();
        IWebDriver? webDriver = null;
        var action = () => webDriver = webDriverFactory.CreateDriver();
        action.ShouldNotThrow();
        webDriver.ShouldNotBeNull();
        webDriver.Navigate().GoToUrl("https://www.google.com");
        webDriver.Title.Contains("Google").ShouldBeTrue();

        var count0 = webDriver.WindowHandles.Count;
        var handle0 = webDriver.CurrentWindowHandle;


        var tab1 = webDriver.SwitchTo().NewWindow(WindowType.Tab).SwitchTo().NewWindow(WindowType.Tab);
        var count1 = webDriver.WindowHandles.Count;
        var handle1 = webDriver.CurrentWindowHandle;
        webDriver.Navigate().GoToUrl("https://www.qwant.fr");
        var toto = webDriver.SwitchTo().Window(handle0);

        tab1.Navigate().GoToUrl("https://www.yahoo.fr");


        webDriver.Close();
        webDriver.Dispose();
    }
}