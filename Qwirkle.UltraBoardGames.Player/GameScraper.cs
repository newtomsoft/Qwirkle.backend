namespace Qwirkle.UltraBoardGames.Player;

public class GameScraper : IDisposable
{
    private const int TilesNumberPerPlayer = 6;
    private const string GamePageUrlPlaySecond = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
    private const string GamePageUrlPlayFirst = "https://www.ultraboardgames.com/qwirkle/game.php?startplayer";
    private static string QwirkleGamePageWithRandomFirstPlayer => new Random().Next(2) == 0 ? GamePageUrlPlayFirst : GamePageUrlPlaySecond;
    private readonly IWebDriver _webDriver;
    private readonly ILogger<UltraBoardGamesPlayerApplication> _logger;
    private string _pathToGameImages = string.Empty;

    public GameScraper(ILogger<UltraBoardGamesPlayerApplication> logger, IWebDriverFactory webDriverFactory)
    {
        _logger = logger;
        _webDriver = webDriverFactory.CreateDriver();
        _ = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
    }

    public void GoToGame()
    {
        _pathToGameImages = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss");
        Directory.CreateDirectory(_pathToGameImages);
        _webDriver.Navigate().GoToUrl(QwirkleGamePageWithRandomFirstPlayer);
        _webDriver.Manage().Window.Maximize();
    }

    public GameStatus GetGameStatus()
    {
        try
        {
            var elementEndGame = _webDriver.FindElement(By.Id("messageboard"));
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
        var limitDate = DateTime.Now.AddSeconds(2);
        while (DateTime.Now < limitDate)
        {
            try
            {
                _webDriver.FindElement(By.Id("ez-accept-all")).Click();
                break;
            }
            catch
            {
                // ignored
            }
        }
    }

    public int GetTilesOnBag()
    {
        var brutTilesOnBag = _webDriver.FindElement(By.Id("bag_status")).Text;
        return ConvertToInt(brutTilesOnBag);
    }

    public int GetPlayerPoints()
    {
        var brutPoints = _webDriver.FindElement(By.Id("score_player")).Text;
        return ConvertToInt(brutPoints, '(');
    }

    public int GetOpponentPoints()
    {
        var brutPoints = _webDriver.FindElement(By.Id("score_computer")).Text;
        return ConvertToInt(brutPoints, '(');
    }

    public List<TileOnPlayer> GetTilesOnPlayer()
    {
        var tilesCodes = GetTilesOnPlayerCodes();
        return tilesCodes.Select(positionTile => new TileOnPlayer(positionTile.Key, positionTile.Value.ToTile())).ToList();
    }


    /// <returns>0 tiles if error</returns>
    public HashSet<TileOnBoard> GetTilesPlayedByOpponent()
    {
        var tilesOnBoard = new HashSet<TileOnBoard>();
        var domImageTiles = _webDriver.FindElements(By.ClassName("lastotherplayer")).ToList();
        foreach (var domImageTile in domImageTiles)
        {
            try
            {
                var fullImageName = domImageTile.GetAttribute("src");
                var domTile = domImageTile.FindElement(By.XPath("./.."));
                var coordinates = Coordinates.From(int.Parse(domTile.GetAttribute("mapx")), int.Parse(domTile.GetAttribute("mapy")));
                tilesOnBoard.Add(TileOnBoard.From(GetImageCode(fullImageName).ToTile(), coordinates));
            }
            catch (Exception exception)
            {
                _logger.LogError("{applicationEvent} in {methodName} {message} at {dateTime}", "Exception", System.Reflection.MethodBase.GetCurrentMethod()!.Name, exception.Message, DateTime.UtcNow);
                return new HashSet<TileOnBoard>();
            }
        }
        return tilesOnBoard;
    }

    public List<TileOnBoard> GetTilesOnBoard()
    {
        var tilesOnBoard = new List<TileOnBoard>();
        var domTiles = _webDriver.FindElements(By.ClassName("ui-droppable"))/*.Where(e => e.GetAttribute("Onclick") is null)*/.ToList();
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
                element = _webDriver.FindElements(By.XPath("//*[contains(., 'Computer:3')]"))[^1];
                break;
            }
            catch
            {
                break;
            }
        }
    }

    public void Play(List<TileOnBoard>? tilesToPlay)
    {
        if (tilesToPlay is null || tilesToPlay.Count == 0)
        {
            Skip(); //todo test swap tiles / all tiles ?
            return;
        }
        PlaceTilesOnBoard(tilesToPlay);
        ClickPlay();
    }

    public void Swap(int number)
    {
        PlaceTilesOnBag(number);
        ClickPlay();
    }

    private Dictionary<RackPosition, string> GetTilesOnPlayerCodes()
    {
        var tilesCodes = new Dictionary<RackPosition, string>();
        for (var i = 0; i < TilesNumberPerPlayer; i++)
        {
            try
            {
                var fullImageName = _webDriver.FindElement(By.Id($"d{i}")).FindElement(By.TagName("img")).GetAttribute("src");
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
        ((IJavaScriptExecutor)_webDriver).ExecuteScript("Qwirkle.passConfirmation(1)");
    }

    private void ClickPlay()
    {
        var notDone = true;
        while (notDone)
        {
            try
            {
                _webDriver.FindElement(By.Id("okay")).Click();
                notDone = false;
            }
            catch (Exception exception)
            {
                _logger.LogError("{applicationEvent} in {methodName} {message} at {dateTime}", "Exception", System.Reflection.MethodBase.GetCurrentMethod()!.Name, exception.Message, DateTime.UtcNow);
                ((IJavaScriptExecutor)_webDriver).ExecuteScript("document.getElementById('ezmobfooter').style.display='none';");
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
            try
            {
                DragAndDrop(elementFrom!, elementTo);
                FindElementMoveFrom(tile, playerTilesCodes);
                LogFindElementMoveFrom(tile, coordinates);
            }
            catch (Exception exception)
            {
                _logger.LogError("{applicationEvent} in {methodName} {message} at {dateTime}", "Exception", System.Reflection.MethodBase.GetCurrentMethod()!.Name, exception.Message, DateTime.UtcNow);
                Task.Delay(100);
                PlaceTilesOnBoard(tiles);
            }
        }
    }

    private void PlaceTilesOnBag(int number)
    {
        var elementTo = _webDriver.FindElement(By.Id("d1000"));
        for (var iTile = 0; iTile < number; iTile++)
        {
            var elementFrom = FindElementMoveFrom(iTile);
            try
            {
                DragAndDrop(elementFrom!, elementTo);
                FindElementMoveFrom(iTile);
                LogSwapElementMoveFrom(iTile);
            }
            catch (Exception exception)
            {
                _logger.LogError("{applicationEvent} in {methodName} {message} at {dateTime}", "Exception", System.Reflection.MethodBase.GetCurrentMethod()!.Name, exception.Message, DateTime.UtcNow);
                Task.Delay(100);
                PlaceTilesOnBag(number);
            }
        }
    }

    private IWebElement? FindElementMoveFrom(Tile tile, Dictionary<RackPosition, string> playerTilesCodes)
    {
        var tileCode = tile.ToCode();
        var indexTile = playerTilesCodes.First(e => e.Value == tileCode).Key;
        return FindElementMoveFrom(indexTile);
    }

    private IWebElement? FindElementMoveFrom(int indexTile)
    {
        try
        {
            return _webDriver.FindElement(By.Id($"d{indexTile}"));
        }
        catch (Exception exception)
        {
            _logger.LogError("{applicationEvent} in {methodName} {message} at {dateTime}", "Exception", System.Reflection.MethodBase.GetCurrentMethod()!.Name, exception.Message, DateTime.UtcNow);
            return null;
        }
    }

    private IWebElement FindElementMoveTo(Coordinates coordinates) => _webDriver.FindElement(By.Id($"x{coordinates.X}y{coordinates.Y}"));

    private void DragAndDrop(IWebElement fromElement, IWebElement toElement) => new Actions(_webDriver).DragAndDrop(fromElement, toElement).Build().Perform();

    private static int ConvertToInt(string brutPoints, char ignoreAfter = ' ')
    {
        var indexToRemoveAfter = brutPoints.IndexOf(ignoreAfter);
        var pointsString = indexToRemoveAfter > 0 ? brutPoints.Remove(indexToRemoveAfter) : brutPoints;
        return int.Parse(pointsString);
        // brutPoints : "10 (4 last turn)" -> return 10
    }
    private static string GetImageCode(string fullImageName) => fullImageName[(fullImageName.LastIndexOf('/') + 1)..fullImageName.LastIndexOf('.')];
    public void Dispose() => GC.SuppressFinalize(this);

    public void CloseEndWindow()
    {
        try
        {
            var inputNameElement = _webDriver.FindElement(By.Id("nicknamepop"));
            inputNameElement.SendKeys("newtom");
        }
        catch
        {
            // ignored
        }
        _webDriver.FindElement(By.ClassName("messageboard_button")).Click();
        ((IJavaScriptExecutor)_webDriver).ExecuteScript("Qwirkle.removeMessageBoard()");
        TakeScreenShot();
    }

    [SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme", Justification = "<En attente>")]
    public void TakeScreenShot()
    {
        const int margin = 5;
        var boardElement = _webDriver.FindElement(By.Id("board"));
        var scoresElement = _webDriver.FindElement(By.Id("scores"));
        var rackElement = _webDriver.FindElement(By.Id("scoreboard1"));
        var totalHeight = boardElement.Size.Height + scoresElement.Size.Height + rackElement.Size.Height;
        var totalWidth = boardElement.Size.Width;
        var size = new Size(totalWidth + 2 * margin, totalHeight + 2 * margin);
        var originLocation = scoresElement.Location;
        originLocation.Offset(-margin, -margin);
        var screenShot = ((ITakesScreenshot)_webDriver).GetScreenshot();
        //screenShot.SaveAsFile(screenShotFilename, ScreenshotImageFormat.Png);
        using var memStream = new MemoryStream(screenShot.AsByteArray);
        var screenShotImage = Image.FromStream(memStream);
        var bitmap = CropImage(screenShotImage, new Rectangle(originLocation, size));
        var screenShotFilename = Path.Combine(_pathToGameImages, DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss") + ".png");
        bitmap.Save(screenShotFilename, ImageFormat.Png);

        static Bitmap CropImage(Image originImage, Rectangle sourceRectange)
        {
            var destRect = new Rectangle(Point.Empty, sourceRectange.Size);
            var cropImage = new Bitmap(destRect.Width, destRect.Height);
            using var graphics = Graphics.FromImage(cropImage);
            graphics.DrawImage(originImage, destRect, sourceRectange, GraphicsUnit.Pixel);
            return cropImage;
        }
    }

    private void LogFindElementMoveFrom(Tile tile, Coordinates coordinates) => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} {tile} to {coordinates} at {dateTime}", "Move elements", tile, coordinates, DateTime.UtcNow);
    private void LogSwapElementMoveFrom(int tileIndex) => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} {tilePosition} to bag at {dateTime}", "Move elements", tileIndex, DateTime.UtcNow);
}