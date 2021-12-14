namespace Qwirkle.UltraBoardGames.Player;

public class GameScraper : IDisposable
{
    private const string GamePageUrlPlaySecond = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
    private const string GamePageUrlPlayFirst = "https://www.ultraboardgames.com/qwirkle/game.php?startplayer";
    private static string QwirkleGamePageWithRandomFirstPlayer => new Random().Next(2) == 0 ? GamePageUrlPlayFirst : GamePageUrlPlaySecond;
    private readonly IWebDriver _driver;
    private readonly ILogger<UltraBoardGamesPlayerApplication> _logger;
    private string _pathToGameImages = string.Empty;

    public GameScraper(ILogger<UltraBoardGamesPlayerApplication> logger)
    {
        _logger = logger;
        //_driver = new FirefoxDriver();
        _driver = CreateEdgeDriver();
        _ = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
    }

    private static FirefoxDriver CreateFirefoxDriver()
    {
        var profile = new FirefoxProfile();
        profile.AddExtension("ublock_origin-1.39.2-an+fx.xpi");
        //profile.SetPreference("permissions.default.image", 2);
        var options = new FirefoxOptions { Profile = profile };
        var driver = new FirefoxDriver(options);
        return driver;
    }

    private static EdgeDriver CreateEdgeDriver()
    {
        var options = new EdgeOptions();
        options.AddExtensions("uBlock-Origin.edge.crx");
        var driver = new EdgeDriver(options);
        return driver;
    }

    public void GoToGame()
    {
        _pathToGameImages = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss");
        Directory.CreateDirectory(_pathToGameImages);
        _driver.Navigate().GoToUrl(QwirkleGamePageWithRandomFirstPlayer);
        _driver.Manage().Window.Maximize();
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
        var limitDate = DateTime.Now.AddSeconds(2);
        while (DateTime.Now < limitDate)
        {
            try
            {
                _driver.FindElement(By.Id("ez-accept-all")).Click();
                break;
            }
            catch
            {
                Task.Delay(50);
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
            try
            {
                DragAndDrop(elementFrom!, elementTo);
                FindElementMoveFrom(tile, playerTilesCodes);
                LogFindElementMoveFrom(tile, coordinates);
            }
            catch
            {
                //todo parfois un cadre empèche le drag n drop. A gérer
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

    private void DragAndDrop(IWebElement fromElement, IWebElement toElement) => new Actions(_driver).DragAndDrop(fromElement, toElement).Build().Perform();

    private void Swap() => _driver.FindElement(By.Id("okay")).Click();

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
            var inputNameElement = _driver.FindElement(By.Id("nicknamepop"));
            inputNameElement.SendKeys("newtom");
        }
        catch
        {
            // ignored
        }
        _driver.FindElement(By.ClassName("messageboard_button")).Click();
        ((IJavaScriptExecutor)_driver).ExecuteScript("Qwirkle.removeMessageBoard()");
        TakeScreenShot();
    }

    [SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme", Justification = "<En attente>")]
    public void TakeScreenShot()
    {
        const int margin = 5;
        var boardElement = _driver.FindElement(By.Id("board"));
        var scoresElement = _driver.FindElement(By.Id("scores"));
        var rackElement = _driver.FindElement(By.Id("scoreboard1"));
        var totalHeight = boardElement.Size.Height + scoresElement.Size.Height + rackElement.Size.Height;
        var totalWidth = boardElement.Size.Width;
        var size = new Size(totalWidth + 2 * margin, totalHeight + 2 * margin);
        var originLocation = scoresElement.Location;
        originLocation.Offset(-margin, -margin);
        var screenShot = ((ITakesScreenshot)_driver).GetScreenshot();
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
}