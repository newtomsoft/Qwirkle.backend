namespace Qwirkle.Test;

public class ArrangeRackTests
{
    private readonly DefaultDbContext _dbContext;
    private readonly InfoService _infoService;
    private readonly CoreService _service;

    #region arrange methods
    public ArrangeRackTests()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        connectionFactory.Add4DefaultTestUsers();
        var repository = new Repository(_dbContext);
        _infoService = new InfoService(repository, null, new Logger<InfoService>(new LoggerFactory()));
        _service = new CoreService(repository, null, _infoService, null, new Logger<CoreService>(new LoggerFactory()));
    }


    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
    {
        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < 6; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        _dbContext.SaveChanges();
    }
    #endregion


    [Fact]
    public void TryArrangeRackShouldArrangeRackWhenItsPossible()
    {
        var usersIds = _infoService.GetAllUsersId();
        var gameId = _service.CreateGame(usersIds.ToHashSet());
        var players = _infoService.GetGame(gameId).Players;
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Circle && t.Color == TileColor.Green);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Clover && t.Color == TileColor.Blue);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Diamond && t.Color == TileColor.Orange);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.EightPointStar && t.Color == TileColor.Purple);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.FourPointStar && t.Color == TileColor.Red);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Square && t.Color == TileColor.Yellow);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.ToList();

        var playerId = players[0].Id;
        ChangePlayerTilesBy(playerId, constTiles);
        {
            _service.TryArrangeRack(playerId, new List<Tile> { constTile0!.ToTile(), constTile1!.ToTile(), constTile2!.ToTile(), constTile3!.ToTile(), constTile4!.ToTile(), constTile5!.ToTile() });
            var tilesOrderedByPosition = _infoService.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
            for (var i = 0; i < tilesOrderedByPosition.Count; i++)
                tilesOrderedByPosition[i].ToTile().ShouldBe(constTiles[i].ToTile());
        }
        {
            _service.TryArrangeRack(playerId, new List<Tile> { constTile5!.ToTile(), constTile4!.ToTile(), constTile3!.ToTile(), constTile2!.ToTile(), constTile1!.ToTile(), constTile0!.ToTile() });
            var tilesOrderedByPosition = _infoService.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
            for (var i = 0; i < tilesOrderedByPosition.Count; i++)
                tilesOrderedByPosition[i].ToTile().ShouldBe(constTiles[^(i + 1)].ToTile());
        }
        {
            _service.TryArrangeRack(playerId, new List<Tile> { constTile3!.ToTile(), constTile5!.ToTile(), constTile0!.ToTile(), constTile2!.ToTile(), constTile1!.ToTile(), constTile4!.ToTile() });
            var tilesOrderedByPosition = _infoService.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
            tilesOrderedByPosition[0].ToTile().ShouldBe(constTiles[3].ToTile());
            tilesOrderedByPosition[1].ToTile().ShouldBe(constTiles[5].ToTile());
            tilesOrderedByPosition[2].ToTile().ShouldBe(constTiles[0].ToTile());
            tilesOrderedByPosition[3].ToTile().ShouldBe(constTiles[2].ToTile());
            tilesOrderedByPosition[4].ToTile().ShouldBe(constTiles[1].ToTile());
            tilesOrderedByPosition[5].ToTile().ShouldBe(constTiles[4].ToTile());
        }
    }
}