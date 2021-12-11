namespace Qwirkle.UltraBoardGames.Player;

public class GameScraper : IDisposable
{
    private const string GamePageUrlPlaySecond = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
    private const string GamePageUrlPlayFirst = "https://www.ultraboardgames.com/qwirkle/game.php?startplayer";
    private readonly IWebDriver _driver;

    public GameScraper()
    {
        _driver = new FirefoxDriver();
        _ = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var gamePageUrl = new Random().Next(2) == 0 ? GamePageUrlPlayFirst : GamePageUrlPlaySecond;
        _driver.Navigate().GoToUrl(gamePageUrl);
    }
    
    public GameStatus GetGameStatus()
    {
        try
        {
            var elementEndGame = _driver.FindElement(By.Id("messageboard"));
            var text = elementEndGame.FindElement(By.CssSelector("h2")).Text;
            if (text == string.Empty) return GameStatus.InProgress;
            if (text.Contains("You lost")) return GameStatus.Lost;
            if (text.Contains("You won")) return GameStatus.Won;
            return GameStatus.Draw;
        }
        catch
        {
            return GameStatus.InProgress;
        }
    }

    public void AcceptPolicies()
    {
        var limitDate = DateTime.Now.AddSeconds(10);
        while (DateTime.Now < limitDate)
        {
            try
            {
                _driver.FindElement(By.Id("ez-accept-all")).Click();
                break;
            }
            catch
            {
                Task.Delay(100);
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
        var tilesCodes = GetTilesOnPlayerCodes();
        foreach (var position_Tile in tilesCodes) tilesOnPlayer.Add(new TileOnPlayer(position_Tile.Key, position_Tile.Value.ToTile()));
        return tilesOnPlayer;
    }

    public List<TileOnBoard> GetTilesOnBoard()
    {
        var tilesOnBoard = new List<TileOnBoard>();
        var domTiles = _driver.FindElements(By.ClassName("ui-droppable"))/*.Where(e => e.GetAttribute("Onclick") is null)*/.ToList();
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
                // ignored
            }
        }
        return tilesOnBoard;
    }

    internal void GetNotificationOpponentHasPlayed()
    {
        while (true)
        {
            IWebElement element;
            try
            {
                element = _driver.FindElements(By.XPath("//*[contains(., 'Computer:3')]"))[^1];
                break;
            }
            catch
            {
                break;
            }
        }
    }

    public void Play(List<TileOnBoard>? tilesOnBoard)
    {
        if (tilesOnBoard is null || tilesOnBoard.Count == 0)
        {
            Skip(); //todo test swap tiles / all tiles ?
            return;
        }
        PlaceTilesOnBoard(tilesOnBoard);
        ClickPlay();
    }

    private Dictionary<RackPosition, string> GetTilesOnPlayerCodes()
    {
        var tilesCodes = new Dictionary<RackPosition, string>();
        for (var i = 0; i < 6; i++)
        {
            try
            {
                var fullImageName = _driver.FindElement(By.Id($"d{i}")).FindElement(By.TagName("img")).GetAttribute("src");
                tilesCodes.Add((RackPosition)i, GetImageCode(fullImageName));
            }
            catch
            {
                // ignored
            }
        }
        return tilesCodes;
    }

    private void Skip()
    {
        ClickPlay();
        ((IJavaScriptExecutor)_driver).ExecuteScript("Qwirkle.passConfirmation(1)");
    }

    private void ClickPlay()
    {
        var notDone = true;
        while (notDone)
        {
            try
            {
                _driver.FindElement(By.Id("okay")).Click();
                notDone = false;
            }
            catch
            {
                //todo ou diminuer taille board
                ((IJavaScriptExecutor)_driver).ExecuteScript("document.getElementById('ezmobfooter').style.display='none';");
            }
        }
    }

    private void PlaceTilesOnBoard(List<TileOnBoard> tiles)
    {
        var playerTilesCodes = GetTilesOnPlayerCodes();
        foreach (var (tile, coordinates) in tiles)
        {
            var elementFrom = FindElementMoveFrom(tile, playerTilesCodes);
            var elementTo = FindElementMoveTo(coordinates);
            Debug.Assert(elementFrom is not null);
            DragAndDrop(elementFrom, elementTo);
            //todo parfois un cadre empèche le drag n drop.
            //todo à gérer
            try
            {
                FindElementMoveFrom(tile, playerTilesCodes);
            }
            catch
            {
                Debug.Assert(false);
            }
        }
    }

    private IWebElement? FindElementMoveFrom(Tile tile, Dictionary<RackPosition, string> playerTilesCodes)
    {
        var tileCode = tile.ToCode();
        var indexTile = playerTilesCodes.First(e => e.Value == tileCode).Key;
        try
        {
            return _driver.FindElement(By.Id($"d{indexTile}"));
        }
        catch
        {
            Debug.Assert(false);
            return null;
        }
    }

    private IWebElement FindElementMoveTo(Coordinates coordinates) => _driver.FindElement(By.Id($"x{coordinates.X}y{coordinates.Y}"));

    private void DragAndDrop(IWebElement from, IWebElement to)
    {
        var action = new Actions(_driver);
        action.DragAndDrop(from, to).Build().Perform();
    }

    private void Swap()
    {
        _driver.FindElement(By.Id("okay")).Click();
    }

    private static int ConvertToInt(string brutPoints, char ignoreAfter = ' ')
    {
        var indexToRemoveAfter = brutPoints.IndexOf(ignoreAfter);
        var pointsString = indexToRemoveAfter > 0 ? brutPoints.Remove(indexToRemoveAfter) : brutPoints;
        return int.Parse(pointsString);
        // brutPoints : "10 (4 last turn)" -> return 10
    }
    private static string GetImageCode(string fullImageName) => fullImageName[(fullImageName.LastIndexOf('/') + 1)..fullImageName.LastIndexOf('.')];
    public void Dispose() => _driver.Dispose();
}