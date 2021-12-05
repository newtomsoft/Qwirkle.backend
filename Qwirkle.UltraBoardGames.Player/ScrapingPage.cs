using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace Qwirkle.UltraBoardGames.Player;
public class Scraper
{
    private readonly IWebDriver _driver;
    private const string GamePageUrl = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
    private const string DomScorePlayer = "score_player";
    private const string DomScoreBot = "score_computer";
    private const string DomBagStatus = "bag_status";

    public Scraper()
    {
        _driver = new FirefoxDriver();
        _ = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        _driver.Navigate().GoToUrl(GamePageUrl);
    }

    public int GetTilesOnBag()
    {
        //<span id="bag_status">96 tiles left</span>
        var brutTilesOnBag = _driver.FindElement(By.Id(DomBagStatus)).Text;
        return ConvertToInt(brutTilesOnBag);
    }

    public int GetPlayerPoints()
    {
        var brutPoints = _driver.FindElement(By.Id(DomScorePlayer)).Text;
        return ConvertToInt(brutPoints, '(');
    }

    public int GetBotPoints()
    {
        var brutPoints = _driver.FindElement(By.Id(DomScoreBot)).Text;
        return ConvertToInt(brutPoints, '(');
    }


    private static int ConvertToInt(string brutPoints, char ignoreAfter = ' ')
    {
        // brutPoints : "10 (4 last turn)" -> return 10
        var indexToRemoveAfter = brutPoints.IndexOf(ignoreAfter);
        var pointsString = indexToRemoveAfter > 0 ? brutPoints.Remove(indexToRemoveAfter) : brutPoints;
        return int.Parse(pointsString);
    }
}