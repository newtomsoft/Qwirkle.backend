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

    public void AcceptPolicies()
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
                Task.Delay(100);
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

    public List<TileOnPlayer> GetTilesOnPlayer2()
    {
        var tilesOnPlayer = new List<TileOnPlayer>();
        for (int i = 0; i < 6; i++)
        {
            try
            {
                var fullImageName = _driver.FindElement(By.Id($"d{i}")).FindElement(By.TagName("img")).GetAttribute("src");
                tilesOnPlayer.Add(new TileOnPlayer((byte)(i + 1), GetImageCode(fullImageName).ToTile()));
            }
            catch
            { }
        }
        return tilesOnPlayer;
    }

    public List<TileOnPlayer> GetTilesOnPlayer()
    {
        var tilesOnPlayer = new List<TileOnPlayer>();
        var tilesCodes = GetTilesOnPlayerCodes();
        foreach (var position_Tile in tilesCodes) tilesOnPlayer.Add(new TileOnPlayer(position_Tile.Key, position_Tile.Value.ToTile()));
        return tilesOnPlayer;
    }

    private Dictionary<RackPosition, string> GetTilesOnPlayerCodes()
    {
        var tilesCodes = new Dictionary<RackPosition, string>();
        for (int i = 0; i < 6; i++)
        {
            try
            {
                var fullImageName = _driver.FindElement(By.Id($"d{i}")).FindElement(By.TagName("img")).GetAttribute("src");
                tilesCodes.Add((RackPosition)i, GetImageCode(fullImageName));
            }
            catch
            { }
        }
        return tilesCodes;
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

    public void Play(List<TileOnBoard> tilesOnBoard)
    {
        if (tilesOnBoard is null || tilesOnBoard.Count == 0)
        {
            Skip();
            return;
        }
        PlaceTilesOnBoard(tilesOnBoard);
        ClicPlay();
    }

    private void Skip()
    {
        ClicPlay();
        ((IJavaScriptExecutor)_driver).ExecuteScript("Qwirkle.passConfirmation(1)");
    }

    private void ClicPlay() => _driver.FindElement(By.Id("okay")).Click();

    private void PlaceTilesOnBoard(List<TileOnBoard> tiles)
    {
        var playerTilesCodes = GetTilesOnPlayerCodes();
        foreach (var tile in tiles)
        {
            var elementFrom = FindMoveFrom(tile.Tile, playerTilesCodes);
            var elementTo = FindMoveTo(tile.Coordinates);
            DragAndDrop(elementFrom, elementTo);
            try
            {
                if (FindMoveFrom(tile.Tile, playerTilesCodes) is null)
                {
                    Debug.Assert(false);
                }
            }
            catch
            {
                Debug.Assert(false);
            }
        }
    }

    private IWebElement FindMoveFrom(Tile tile, Dictionary<RackPosition, string> playerTilesCodes)
    {
        var tileCode = tile.ToCode();
        var indexTile = playerTilesCodes.First(e => e.Value == tileCode).Key;
        return _driver.FindElement(By.Id($"d{indexTile}"));
    }

    private IWebElement FindMoveTo(Coordinates coordinates) => _driver.FindElement(By.Id($"x{coordinates.X}y{coordinates.Y}"));

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
}