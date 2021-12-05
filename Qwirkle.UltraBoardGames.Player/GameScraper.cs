using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Qwirkle.Domain.ValueObjects;

namespace Qwirkle.UltraBoardGames.Player;
public class GameScraper
{
    private const string GamePageUrl = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";

    private readonly IWebDriver _driver;

    public GameScraper()
    {
        _driver = new FirefoxDriver();
        _ = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        _driver.Navigate().GoToUrl(GamePageUrl);
    }

    public async Task AcceptPoliciesAsync()
    {
        bool isAccepted = false;
        DateTime limitDate = DateTime.Now.AddSeconds(10);
        while (!isAccepted && DateTime.Now < limitDate)
        {
            try
            {
                _driver.FindElement(By.Id("ez-accept-all")).Click();
                break;
            }
            catch
            {
                await Task.Delay(100);
                continue;
            }
        }
    }

    public int GetTilesOnBag()
    {
        var brutTilesOnBag = _driver.FindElement(By.Id("bag_status")).Text;
        return ConvertToInt(brutTilesOnBag);
    }

    public int GetPlayerPoints()
    {
        var brutPoints = _driver.FindElement(By.Id("score_player")).Text;
        return ConvertToInt(brutPoints, '(');
    }

    public int GetOpponentPoints()
    {
        var brutPoints = _driver.FindElement(By.Id("score_computer")).Text;
        return ConvertToInt(brutPoints, '(');
    }

    public List<TileOnPlayer> GetTilesOnPlayer()
    {
        var tilesOnPlayer = new List<TileOnPlayer>();
        for (int i = 0; i < 6; i++)
        {
            var fullImageName = _driver.FindElement(By.Id($"d{i}")).FindElement(By.TagName("img")).GetAttribute("src");
            string imageCode = GetImageCode(fullImageName);
            var tile = imageCode.ToTile();
            tilesOnPlayer.Add(new TileOnPlayer((byte)(i + 1), tile));
        }
        return tilesOnPlayer;
    }

    public List<TileOnBoard> GetTilesOnBoard()
    {
        var tilesOnBoard = new List<TileOnBoard>();
        var domTiles = _driver.FindElements(By.ClassName("ui-droppable")).ToList();
        foreach (var domTile in domTiles)
        {
            try
            {
                var fullImageName = domTile.FindElement(By.TagName("img")).GetAttribute("src");
                var coordinates = Coordinates.From(int.Parse(domTile.GetAttribute("mapx")), int.Parse(domTile.GetAttribute("mapy")));
                tilesOnBoard.Add(TileOnBoard.From(GetImageCode(fullImageName).ToTile(), coordinates));
            }
            catch
            {
            }
        }
        return tilesOnBoard;
    }


    private static int ConvertToInt(string brutPoints, char ignoreAfter = ' ')
    {
        var indexToRemoveAfter = brutPoints.IndexOf(ignoreAfter);
        var pointsString = indexToRemoveAfter > 0 ? brutPoints.Remove(indexToRemoveAfter) : brutPoints;
        return int.Parse(pointsString);
        // brutPoints : "10 (4 last turn)" -> return 10
    }
    private static string GetImageCode(string fullImageName) => fullImageName[(fullImageName.LastIndexOf('/') + 1)..fullImageName.LastIndexOf('.')];
}