namespace Qwirkle.Test;

public class GetDoableMovesShould
{
    private readonly DefaultDbContext _dbContext;
    private readonly CoreUseCase _useCase;

    public GetDoableMovesShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        connectionFactory.Add4DefaultTestUsers();

        var repository = new Repository(_dbContext);
        _useCase = new CoreUseCase(repository, null);
    }

    #region private methods
    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
    {
        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < 6; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void PartWith1TileReturn6MovesWhenBoardIsEmptyWithFirstTileCoordinates0_0Async()
    {
        var usersIds = _useCase.GetAllUsersId();
        var players = _useCase.CreateGame(usersIds);
        players = players.OrderBy(p => p.Id).ToList();
        var constTiles = _dbContext.Tiles.OrderBy(t => t.Id).Take(6).ToList();
        ChangePlayerTilesBy(players[0].Id, constTiles);

        var gameId = players[0].GameId;
        var playReturns = _useCase.GetDoableMoves(gameId, usersIds[0]);

        var playReturnsWith1Tile = playReturns.Where(p => p.TilesPlayed.Count == 1).ToList();
        var tilesPlayedSingle = playReturnsWith1Tile.Select(p => p.TilesPlayed[0]).OrderBy(t => t.Id).ToList();

        playReturnsWith1Tile.Count.ShouldBe(6);
        tilesPlayedSingle.Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0));
        tilesPlayedSingle.Select(t => t.Id).SequenceEqual(constTiles.Select(t => t.Id)).ShouldBeTrue();

        var playReturnsWith2Tiles = playReturns.Where(p => p.TilesPlayed.Count == 2).ToList();
        var tilesPlayed2Tiles = playReturnsWith2Tiles.Select(p => p.TilesPlayed).ToList();

        playReturnsWith2Tiles.Count.ShouldBe(4*5*6);

    }
}