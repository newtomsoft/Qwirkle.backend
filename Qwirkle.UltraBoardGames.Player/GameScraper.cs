namespace Qwirkle.UltraBoardGames.Player;

public sealed class GameScraper
{
    private const string GamePageUrlPlaySecond = "https://www.ultraboardgames.com/qwirkle/game.php?startcomputer";
    private const string GamePageUrlPlayFirst = "https://www.ultraboardgames.com/qwirkle/game.php?startplayer";
    private static string QwirkleGamePageWithRandomFirstPlayer => new Random().Next(2) == 0 ? GamePageUrlPlayFirst : GamePageUrlPlaySecond;
    private readonly ILogger<UltraBoardGamesPlayerApplication> _logger;
    private readonly IWebDriverFactory _webDriverFactory;
    private ScreenShotMaker? _screenShotMaker;
    private IWebDriver _webDriver = null!;
    private IJavaScriptExecutor _javaScriptExecutor = null!;

    public GameScraper(ILogger<UltraBoardGamesPlayerApplication> logger, IWebDriverFactory webDriverFactory)
    {
        _logger = logger;
        _webDriverFactory = webDriverFactory;
    }

    public void BeginScraping()
    {
        _webDriver = _webDriverFactory.CreateDriver();
        _javaScriptExecutor = (IJavaScriptExecutor)_webDriver;
        _ = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
    }

    public void GoToGame()
    {
        _screenShotMaker = new ScreenShotMaker();
        _webDriver.Manage().Window.Size = new System.Drawing.Size(850, 800);
        _webDriver.Navigate().GoToUrl(QwirkleGamePageWithRandomFirstPlayer);
        var wait = FluentWait.Create(_webDriver);
        wait.WithTimeout(TimeSpan.FromMilliseconds(30000));
        wait.PollingInterval = TimeSpan.FromMilliseconds(250);
        wait.Until(IsPageLoaded);
        _logger.LogInformation("Game started");

        bool IsPageLoaded(IWebDriver webDriver)
        {
            try
            {
                _ = _webDriver.FindElement(By.Id("board"));
                _ = _webDriver.FindElement(By.Id("navheader"));
                return true;
            }
            catch
            {
                return false;
            }
        }
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

    public void CleanWindow()
    {
        _javaScriptExecutor.ExecuteScript("document.querySelector('.rightside').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('#ezmobfooter').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('header').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('.breadcrumb').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('.post-title').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('.nextpage').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('.bg-danger').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('footer').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.getElementById('main').style.marginTop = '-65px';");
    }

    public int GetTilesOnBag() => _webDriver.FindElement(By.Id("bag_status")).TextToInt();

    public void AdjustBoardView()
    {

        if (GetGameStatus() != GameStatus.InProgress) return;
        _javaScriptExecutor.ExecuteScript("window.scrollTo(0, 0)");
        var scorePlayer = _webDriver.FindElement(By.Id("score_player"));
        var scoreComputer = _webDriver.FindElement(By.Id("score_player"));
        var scoreBoard = _webDriver.FindElement(By.Id("scoreboard1"));
        var scoreBoard2 = _webDriver.FindElement(By.Id("scoreboard2"));
        var bagStatus = _webDriver.FindElement(By.Id("bag_status"));
        var board = _webDriver.FindElement(By.Id("board"));
        while (!IsVisibleInViewport(scoreBoard) || !IsVisibleInViewport(scoreBoard2) || !IsVisibleInViewport(scorePlayer) || !IsVisibleInViewport(scoreComputer) || !IsVisibleInViewport(bagStatus) || !IsVisibleInViewport(board))
            _javaScriptExecutor.ExecuteScript("Qwirkle.adjustMapSize(0,-1)");
    }

    public int GetPlayerPoints() => _webDriver.FindElement(By.Id("score_player")).TextToInt();
    public int GetPlayerLastPoints() => _webDriver.FindElement(By.Id("score_player")).TextToInt(1);

    public int GetOpponentPoints() => _webDriver.FindElement(By.Id("score_computer")).TextToInt();

    public List<TileOnRack> GetTilesOnPlayer() => TilesOnPlayerCodes().Select(positionTile => new TileOnRack(positionTile.Key, positionTile.Value.ToTile())).ToList();

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
                var coordinates = Coordinate.From(int.Parse(domTile.GetAttribute("mapx")), int.Parse(domTile.GetAttribute("mapy")));
                tilesOnBoard.Add(TileOnBoard.From(GetImageCode(fullImageName).ToTile(), coordinates));
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception in {methodName} {message}", MethodBase.GetCurrentMethod()!.Name, exception.Message);
                return new HashSet<TileOnBoard>();
            }
        }
        return tilesOnBoard;
    }

    public List<TileOnBoard> GetTilesOnBoard()
    {
        var tilesOnBoard = new List<TileOnBoard>();
        var domTiles = _webDriver.FindElements(By.ClassName("ui-droppable")).ToList();
        foreach (var domTile in domTiles)
        {
            try
            {
                var fullImageName = domTile.FindElement(By.TagName("img")).GetAttribute("src");
                var coordinates = Coordinate.From(int.Parse(domTile.GetAttribute("mapx")), int.Parse(domTile.GetAttribute("mapy")));
                tilesOnBoard.Add(TileOnBoard.From(GetImageCode(fullImageName).ToTile(), coordinates));
            }
            catch
            {
                // ignored
            }
        }
        return tilesOnBoard;
    }

    public void Play(List<TileOnBoard> tilesToPlay)
    {
        PlaceTilesOnBoard(tilesToPlay);
        ClickPlay();
    }

    public bool IsPlayingError()
    {
        try
        {
            var messageBoard = _webDriver.FindElement(By.Id("messageboard"));
            var text = messageBoard.FindElement(By.CssSelector("h3")).Text;
            if (text.Contains("You havn't played any tiles!") is not true) return false;
            _webDriver.FindElement(By.ClassName("messageboard_button")).Click();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Swap(int number)
    {
        PlaceTilesOnBag(number);
        ClickPlay();
    }

    public void Skip()
    {
        ClickPlay();
        _javaScriptExecutor.ExecuteScript("Qwirkle.passConfirmation(1)");
    }

    public void CloseEndGameNotificationWindow()
    {
        while (true)
        {
            try
            {
                var inputNameElement = _webDriver.FindElement(By.Id("nicknamepop"));
                inputNameElement.SendKeys(RandomNickName());
                break;
            }
            catch
            {
                // ignored
            }
        }
        _webDriver.FindElement(By.ClassName("messageboard_button")).Click();
        _javaScriptExecutor.ExecuteScript("Qwirkle.removeMessageBoard()");
        TakeScreenShot();
    }

    public void TakeScreenShot()
    {
        var elements = new List<IWebElement> { _webDriver.FindElement(By.Id("board")), _webDriver.FindElement(By.Id("scoreboard1")), _webDriver.FindElement(By.Id("score_computer_div")) };
        _screenShotMaker!.SaveCroppedScreenShot(GetByteArrayScreenShot, elements);

        byte[] GetByteArrayScreenShot() => ((ITakesScreenshot)_webDriver).GetScreenshot().AsByteArray;
    }

    public void CloseBrowser(TimeSpan delay)
    {
        Task.Delay(delay).Wait();
        _webDriver.Close();
        _webDriver.Dispose();
    }

    private Dictionary<RackPosition, UltraBoardGamesTileImageCode> TilesOnPlayerCodes()
    {
        var tilesCodes = new Dictionary<RackPosition, UltraBoardGamesTileImageCode>();
        for (var i = 0; i < CoreService.TilesNumberPerPlayer; i++)
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
                _logger?.LogError("Exception in {methodName} {message}", MethodBase.GetCurrentMethod()!.Name, exception.Message);
                _javaScriptExecutor.ExecuteScript("document.getElementById('ezmobfooter').style.display='none';");
            }
        }
    }

    private void PlaceTilesOnBoard(List<TileOnBoard> tiles)
    {
        var playerTilesCodes = TilesOnPlayerCodes();
        foreach (var tile in tiles)
        {
            var elementFrom = FindElementMoveFrom(tile, playerTilesCodes);
            var elementTo = FindElementMoveTo(tile.Coordinate);
            try
            {
                DragAndDrop(elementFrom!, elementTo);
                FindElementMoveFrom(tile, playerTilesCodes);
                LogMoveTile(tile);
            }
            catch (Exception exception)
            {
                _logger?.LogError("Exception in {methodName} {message}", MethodBase.GetCurrentMethod()!.Name, exception.Message);
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
                _logger?.LogError("Exception in {methodName} {message}", MethodBase.GetCurrentMethod()!.Name, exception.Message);
                Task.Delay(100);
                PlaceTilesOnBag(number);
            }
        }
    }

    private IWebElement? FindElementMoveFrom(Tile tile, Dictionary<RackPosition, UltraBoardGamesTileImageCode> playerTilesCodes)
    {
        var tileCode = tile.ToCode();
        var indexTile = playerTilesCodes.FirstOrDefault(e => e.Value == tileCode).Key;
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
            _logger.LogError("Exception in {methodName} {message}", MethodBase.GetCurrentMethod()!.Name, exception.Message);
            return null;
        }
    }

    private IWebElement FindElementMoveTo(Coordinate coordinate) => _webDriver.FindElement(By.Id($"x{coordinate.X}y{coordinate.Y}"));

    private void DragAndDrop(IWebElement fromElement, IWebElement toElement) => new Actions(_webDriver).DragAndDrop(fromElement, toElement).Build().Perform();
    private static UltraBoardGamesTileImageCode GetImageCode(string fullImageName) => new(GetStringImageCode(fullImageName));
    private static string GetStringImageCode(string fullImageName) => fullImageName[(fullImageName.LastIndexOf('/') + 1)..fullImageName.LastIndexOf('.')];

    private static string RandomNickName()
    {
        var nickNames = new List<string> { "newtom", "newtomsoft", "thomas", "tom" };
        var randIndex = new Random().Next(nickNames.Count);
        var randSuffix = new Random().Next(10, 100);
        return nickNames[randIndex] + randSuffix;
    }

    private void LogMoveTile(TileOnBoard tile) => _logger.LogInformation("Player move {tile}", tile);
    private void LogSwapElementMoveFrom(int tileIndex) => _logger.LogInformation("Move elements {tilePosition} to bag", tileIndex);
    private bool IsVisibleInViewport(IWebElement element) => (bool)_javaScriptExecutor.ExecuteScript("var elem = arguments[0], box = elem.getBoundingClientRect(), cx = box.left + box.width / 2,  cy = box.top + box.height / 2,  e = document.elementFromPoint(cx, cy); for (; e; e = e.parentElement) { if (e === elem) return true;} return false;", element);


}