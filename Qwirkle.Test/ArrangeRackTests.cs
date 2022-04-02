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
        var tilesOnPlayer = _dbContext.TilesOnRack.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < 6; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void TryArrangeRackShouldNotArrangeRackWhenATileIsNotInPlayerRack()
    {
        var usersIds = _infoService.GetAllUsersId();
        var gameId = _service.CreateGame(usersIds.ToHashSet());
        var players = _infoService.GetGame(gameId).Players;
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.First(t => t.Shape == TileShape.Circle && t.Color == TileColor.Green);
        var constTile1 = _dbContext.Tiles.First(t => t.Shape == TileShape.Clover && t.Color == TileColor.Blue);
        var constTile2 = _dbContext.Tiles.First(t => t.Shape == TileShape.Diamond && t.Color == TileColor.Orange);
        var constTile3 = _dbContext.Tiles.First(t => t.Shape == TileShape.EightPointStar && t.Color == TileColor.Purple);
        var constTile4 = _dbContext.Tiles.First(t => t.Shape == TileShape.FourPointStar && t.Color == TileColor.Red);
        var constTile5 = _dbContext.Tiles.First(t => t.Shape == TileShape.Square && t.Color == TileColor.Yellow);
        var constTiles = new List<TileDao> { constTile0, constTile1, constTile2, constTile3, constTile4, constTile5 }.ToList();

        var badTile = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Circle && t.Color == TileColor.Blue);

        var playerId = players[0].Id;
        ChangePlayerTilesBy(playerId, constTiles);

        var arrangeRackReturn = _service.TryArrangeRack(playerId, new List<TileOnRack> { badTile.ToTileOnRack(0), constTile1.ToTileOnRack(1), constTile2.ToTileOnRack(2), constTile3.ToTileOnRack(3), constTile4.ToTileOnRack(4), constTile5.ToTileOnRack(5) });
        arrangeRackReturn.ShouldBe(new ArrangeRackReturn(ReturnCode.PlayerDoesntHaveThisTile));

    }

    [Fact]
    public void TryArrangeRackShouldArrangeRackWhenItsPossible()
    {
        var usersIds = _infoService.GetAllUsersId();
        var gameId = _service.CreateGame(usersIds.ToHashSet());
        var players = _infoService.GetGame(gameId).Players;
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.First(t => t.Shape == TileShape.Circle && t.Color == TileColor.Green);
        var constTile1 = _dbContext.Tiles.First(t => t.Shape == TileShape.Clover && t.Color == TileColor.Blue);
        var constTile2 = _dbContext.Tiles.First(t => t.Shape == TileShape.Diamond && t.Color == TileColor.Orange);
        var constTile3 = _dbContext.Tiles.First(t => t.Shape == TileShape.EightPointStar && t.Color == TileColor.Purple);
        var constTile4 = _dbContext.Tiles.First(t => t.Shape == TileShape.FourPointStar && t.Color == TileColor.Red);
        var constTile5 = _dbContext.Tiles.First(t => t.Shape == TileShape.Square && t.Color == TileColor.Yellow);
        var constTiles = new List<TileDao> { constTile0, constTile1, constTile2, constTile3, constTile4, constTile5 }.ToList();

        var playerId = players[0].Id;
        ChangePlayerTilesBy(playerId, constTiles);
        {
            _service.TryArrangeRack(playerId, new List<TileOnRack> { constTile0.ToTileOnRack(0), constTile1.ToTileOnRack(1), constTile2.ToTileOnRack(2), constTile3.ToTileOnRack(3), constTile4.ToTileOnRack(4), constTile5.ToTileOnRack(5) });
            var tilesOrderedByPosition = _infoService.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
            for (var i = 0; i < tilesOrderedByPosition.Count; i++)
                tilesOrderedByPosition[i].ToTile().ShouldBe(constTiles[i].ToTile());
        }
        {
            _service.TryArrangeRack(playerId, new List<TileOnRack> { constTile5.ToTileOnRack(0), constTile4.ToTileOnRack(1), constTile3.ToTileOnRack(2), constTile2.ToTileOnRack(3), constTile1.ToTileOnRack(4), constTile0.ToTileOnRack(5) });
            var tilesOrderedByPosition = _infoService.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
            for (var i = 0; i < tilesOrderedByPosition.Count; i++)
                tilesOrderedByPosition[i].ToTileOnRack((byte)(i)).ShouldBe(constTiles[^(i + 1)].ToTileOnRack((byte)i));
        }
        {
            _service.TryArrangeRack(playerId, new List<TileOnRack> { constTile3.ToTileOnRack(0), constTile5.ToTileOnRack(1), constTile0.ToTileOnRack(2), constTile2.ToTileOnRack(3), constTile1.ToTileOnRack(4), constTile4.ToTileOnRack(5) });
            var tilesOrderedByPosition = _infoService.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
            tilesOrderedByPosition[0].ToTileOnRack(0).ShouldBe(constTiles[3].ToTileOnRack(0));
            tilesOrderedByPosition[1].ToTile().ShouldBe(constTiles[5].ToTile());
            tilesOrderedByPosition[2].ToTile().ShouldBe(constTiles[0].ToTile());
            tilesOrderedByPosition[3].ToTile().ShouldBe(constTiles[2].ToTile());
            tilesOrderedByPosition[4].ToTile().ShouldBe(constTiles[1].ToTile());
            tilesOrderedByPosition[5].ToTile().ShouldBe(constTiles[4].ToTile());
        }
    }
}