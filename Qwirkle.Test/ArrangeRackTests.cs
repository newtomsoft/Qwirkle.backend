//namespace Qwirkle.Test;

//public class ArrangeRackTests
//{
//    private readonly DefaultDbContext _dbContext;
//    private readonly InfoUseCase _infoUseCase;
//    private readonly CoreUseCase _useCase;

//    #region arrange methods
//    public ArrangeRackTests()
//    {
//        var connectionFactory = new ConnectionFactory();
//        _dbContext = connectionFactory.CreateContextForInMemory();
//        connectionFactory.Add4DefaultTestUsers();

//        var repository = new Repository(_dbContext);
//        _infoUseCase = new InfoUseCase(repository, null);
//        _useCase = new CoreUseCase(repository, null, _infoUseCase);
//    }


//    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
//    {
//        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
//        for (var i = 0; i < 6; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
//        _dbContext.SaveChanges();
//    }
//    #endregion


//    [Fact]
//    public void TryArrangeRackShouldArrangeRackWhenItsPossible()
//    {
//        var usersIds = _infoUseCase.GetAllUsersId();
//        var players = _useCase.CreateGame(usersIds);
//        players = players.OrderBy(p => p.Id).ToList();
//        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Circle && t.Color == TileColor.Green);
//        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Clover && t.Color == TileColor.Blue);
//        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Diamond && t.Color == TileColor.Orange);
//        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.EightPointStar && t.Color == TileColor.Purple);
//        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.FourPointStar && t.Color == TileColor.Red);
//        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Shape == TileShape.Square && t.Color == TileColor.Yellow);
//        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.ToList();

//        var playerId = players[0].Id;
//        ChangePlayerTilesBy(playerId, constTiles);

//        {
//            _useCase.TryArrangeRack(playerId, new List<int> { constTile0!.Id, constTile1!.Id, constTile2!.Id, constTile3!.Id, constTile4!.Id, constTile5!.Id });
//            var tilesOrderedByPosition = _infoUseCase.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
//            for (var i = 0; i < tilesOrderedByPosition.Count; i++)
//                tilesOrderedByPosition[i].ToTile().ShouldBe(constTiles[i].ToTile());
//        }
//        {
//            _useCase.TryArrangeRack(playerId, new List<int> { constTile5.Id, constTile4.Id, constTile3.Id, constTile2.Id, constTile1.Id, constTile0.Id });
//            var tilesOrderedByPosition = _infoUseCase.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
//            for (var i = 0; i < tilesOrderedByPosition.Count; i++)
//                tilesOrderedByPosition[i].ToTile().ShouldBe(constTiles[^(i + 1)].ToTile());
//        }
//        {
//            _useCase.TryArrangeRack(playerId, new List<int> { constTile3.Id, constTile5.Id, constTile0.Id, constTile2.Id, constTile1.Id, constTile4.Id });
//            var tilesOrderedByPosition = _infoUseCase.GetPlayer(playerId).Rack.Tiles.OrderBy(t => t.RackPosition).ToList();
//            tilesOrderedByPosition[0].ToTile().ShouldBe(constTiles[3].ToTile());
//            tilesOrderedByPosition[1].ToTile().ShouldBe(constTiles[5].ToTile());
//            tilesOrderedByPosition[2].ToTile().ShouldBe(constTiles[0].ToTile());
//            tilesOrderedByPosition[3].ToTile().ShouldBe(constTiles[2].ToTile());
//            tilesOrderedByPosition[4].ToTile().ShouldBe(constTiles[1].ToTile());
//            tilesOrderedByPosition[5].ToTile().ShouldBe(constTiles[4].ToTile());
//        }

//    }
//}