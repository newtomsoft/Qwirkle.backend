namespace Qwirkle.Infra.Repository.Adapters;
public class Repository : IRepository
{
    private DefaultDbContext DbContext { get; }

    private const int TOTAL_TILES = 108;

    public Repository(DefaultDbContext defaultDbContext)
    {
        DbContext = defaultDbContext;
    }

    public void CreateTiles(int gameId)
    {
        AddAllTilesInDataBase();
        var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
        for (int i = 0; i < TOTAL_TILES; i++)
            DbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tilesIds[i] });

        DbContext.SaveChanges();
    }

    public Player CreatePlayer(int userId, int gameId)
    {
        var playerDao = new PlayerDao { GameId = gameId, UserId = userId, LastTurnSkipped = false };
        DbContext.Players.Add(playerDao);
        DbContext.SaveChanges();
        return PlayerDaoToPlayer(playerDao);
    }

    public Game CreateGame(DateTime date)
    {
        var game = new GameDao { CreatDate = date.ToUniversalTime() };
        DbContext.Games.Add(game);
        DbContext.SaveChanges();
        return GameDaoToGame(game);
    }
    public List<int> GetListGameIDWithPlayer() => DbContext.Players.Select(p => p.GameId).Distinct().ToList();

    public List<int> GetUsersId() => DbContext.Users.Select(u => u.Id).ToList();

    public List<int> GetUserGames(int userId) => DbContext.Players.Where(p => p.UserId == userId).Select(p => p.GameId).ToList();

    public List<string> GetListNamePlayer(int gameId)
    {
        var listName = new List<string>();
        var players = DbContext.Players.Where(p => p.GameId == gameId).ToList();
        players.ForEach(player => listName.Add(DbContext.Users.Where(u => u.Id == player.UserId).Select(u => u.UserName).FirstOrDefault()));
        return listName;
    }

    public string GetPlayerNameTurn(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Include(p => p.User).FirstOrDefault()?.User.UserName;

    public int GetPlayerIdToPlay(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Select(p => p.Id).FirstOrDefault();

    public Tile GetTileById(int tileId) => TileDaoToTile(DbContext.Tiles.Single(t => t.Id == tileId));

    public TileOnPlayer GetTileOnPlayerById(int playerId, int tileId) => TileOnPlayerDaoToEntity(DbContext.TilesOnPlayer.Single(t => t.PlayerId == playerId && t.TileId == tileId));

    public Game GetGame(int gameId)
    {
        var gameDao = DbContext.Games.Where(g => g.Id == gameId).Include(g => g.Players).ThenInclude(p => p.User).Include(g => g.Players).ThenInclude(p => p.Tiles).ThenInclude(t => t.Tile).Include(g => g.TilesOnBoard).ThenInclude(t => t.Tile).Include(g => g.TilesOnBag).ThenInclude(t => t.Tile).FirstOrDefault();
        var tilesOnBoard = TilesOnBoardDaoToEntity(gameDao.TilesOnBoard);
        var players = new List<Player>();
        gameDao.Players.ForEach(player => players.Add(PlayerDaoToPlayerFork(player)));
        var bag = new Bag(gameId);
        gameDao.TilesOnBag.ForEach(t => bag.Tiles.Add(TileOnBagDaoToEntity(t)));
        return new Game(gameDao.Id, tilesOnBoard, players, gameDao.GameOver, bag);
    }

    public Player GetPlayer(int playerId) => PlayerDaoToPlayer(DbContext.Players.Where(p => p.Id == playerId).Include(p => p.User).FirstOrDefault());
    public Player GetPlayer(int gameId, int userId) => PlayerDaoToPlayer(DbContext.Players.Where(p => p.GameId == gameId && p.UserId == userId).FirstOrDefault());

    public void UpdatePlayer(Player player)
    {
        DbContext.Players.Update(PlayerToPlayerDaoWithoutTile(player));
        DbContext.SaveChanges();
    }

    public void ArrangeRack(Player player, List<TileOnPlayer> tilesToArrange)
    {
        for (byte i = 0; i < tilesToArrange.Count; i++)
        {
            var tile = DbContext.TilesOnPlayer.Single(tp => tp.PlayerId == player.Id && tp.TileId == tilesToArrange[i].Id);
            tile.RackPosition = i;
        }
        DbContext.SaveChanges();
    }

    public void TilesFromBagToPlayer(Player player, List<byte> positionsInRack)
    {
        int tilesNumber = positionsInRack.Count;
        var tilesToGiveToPlayer = DbContext.TilesOnBag.Where(t => t.GameId == player.GameId).ToList().OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
        DbContext.TilesOnBag.RemoveRange(tilesToGiveToPlayer);
        for (int i = 0; i < tilesToGiveToPlayer.Count; i++)
            DbContext.TilesOnPlayer.Add(new TileOnPlayerDao(tilesToGiveToPlayer[i], positionsInRack[i], player.Id));
        DbContext.SaveChanges();
    }

    public void TilesFromPlayerToBag(Player player, List<TileOnPlayer> tiles)
    {
        var game = DbContext.Games.Single(g => g.Id == player.GameId);
<<<<<<< HEAD
        game.LastPlayDate = DateTime.UtcNow;
=======
        game.LastPlayDate = DateTime.Now.ToUniversalTime();
>>>>>>> 7667b4821d1ddc82f50431964dabe6af2b0173be
        var tilesOnPlayer = DbContext.TilesOnPlayer.Where(t => t.PlayerId == player.Id && tiles.Select(t => t.Id).Contains(t.TileId)).ToList();
        DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayer);
        tilesOnPlayer.ForEach(tp => DbContext.TilesOnBag.Add(TileOnPlayerDaoToTileOnBagDao(tp, player.GameId)));
        DbContext.SaveChanges();
    }

    public void TilesFromPlayerToGame(int gameId, int playerId, List<TileOnBoard> tiles)
    {
        var game = DbContext.Games.Single(g => g.Id == gameId);
<<<<<<< HEAD
        game.LastPlayDate = DateTime.UtcNow;
=======
        game.LastPlayDate = DateTime.Now.ToUniversalTime();
>>>>>>> 7667b4821d1ddc82f50431964dabe6af2b0173be
        tiles.ForEach(t => DbContext.TilesOnBoard.Add(TileToTileOnBoardDao(t, gameId)));
        tiles.ForEach(t => DbContext.TilesOnPlayer.Remove(DbContext.TilesOnPlayer.Single(tp => tp.TileId == t.Id && tp.PlayerId == playerId)));
        DbContext.SaveChanges();
    }

    public void SetPlayerTurn(int playerId)
    {
        var player = DbContext.Players.Single(p => p.Id == playerId);
        player.GameTurn = true;
        DbContext.SaveChanges();
    }

    public void SetGameOver(int gameId)
    {
        foreach (var player in DbContext.Players.Where(p => p.GameId == gameId)) player.GameTurn = false;
        DbContext.Games.Single(g => g.Id == gameId).GameOver = true;
        DbContext.SaveChanges();
    }

    public List<int> GetLeadersPlayersId(int gameId) => DbContext.Players.Where(p => p.GameId == gameId && p.Points == DbContext.Players.Where(p => p.GameId == gameId).Max(p => p.Points)).Select(p => p.Id).ToList();

    public bool IsGameOver(int gameId) => DbContext.Games.Where(g => g.Id == gameId && g.GameOver).Any();

    private void AddAllTilesInDataBase()
    {
        if (DbContext.Tiles.Count() == TOTAL_TILES) return;

        const int NUMBER_OF_SAME_TILE = 3;
        for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    DbContext.Tiles.Add(new TileDao { Color = color, Shape = shape });

        DbContext.SaveChanges();
    }

    private List<TileOnBoard> TilesOnBoardDaoToEntity(List<TileOnBoardDao> tilesOnBoard)
    {
        var tiles = new List<TileOnBoard>();
        var tilesModel = DbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
        foreach (var tileModel in tilesModel)
        {
            var tileOnGame = tilesOnBoard.Single(tb => tb.TileId == tileModel.Id);
            tiles.Add(new TileOnBoard(tileModel.Id, tileModel.Color, tileModel.Shape, new CoordinatesInGame(tileOnGame.PositionX, tileOnGame.PositionY)));
        }
        return tiles;
    }

    private PlayerDao PlayerToPlayerDaoWithoutTile(Player player)
    {
        var playerDao = DbContext.Players.Single(gp => gp.Id == player.Id);
        playerDao.Points = (byte)player.Points;
        playerDao.LastTurnPoints = (byte)player.LastTurnPoints;
        playerDao.GameTurn = player.IsTurn;
        playerDao.GamePosition = (byte)player.GamePosition;
        playerDao.LastTurnSkipped = player.LastTurnSkipped;
        return playerDao;
    }

    private Player PlayerDaoToPlayer(PlayerDao playerDao)
    {
        var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerDao.Id).Include(t => t.Tile).Include(t => t.Player.User).ToList();
        var tiles = new List<TileOnPlayer>();
        tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerDaoToEntity(tp)));
        var player = new Player(playerDao.Id, playerDao.GameId, playerDao.User?.UserName, playerDao.GamePosition, playerDao.Points, playerDao.LastTurnPoints, tiles, playerDao.GameTurn, playerDao.LastTurnSkipped);
        return player;
    }
    private Player PlayerDaoToPlayerFork(PlayerDao playerDao)
    {
        var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerDao.Id).ToList();
        var tiles = new List<TileOnPlayer>();
        tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerDaoToEntity(tp)));
        var player = new Player(playerDao.Id, playerDao.GameId, playerDao.User.UserName, playerDao.GamePosition, playerDao.Points, playerDao.LastTurnPoints, tiles, playerDao.GameTurn, playerDao.LastTurnSkipped);
        return player;
    }

    private static TileOnBoardDao TileToTileOnBoardDao(TileOnBoard tile, int gameId) => new() { TileId = tile.Id, GameId = gameId, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y };

    private TileOnPlayerDao TileOnBagToTileOnPlayer(TileOnBagDao tileOnBag, int playerId) => new() { TileId = tileOnBag.TileId, PlayerId = playerId };

    private static TileOnBagDao TileOnPlayerDaoToTileOnBagDao(TileOnPlayerDao tileOnPlayer, int gameId) => new() { TileId = tileOnPlayer.TileId, GameId = gameId };

    private static TileOnPlayer TileOnPlayerDaoToEntity(TileOnPlayerDao tileOnPlayer) => new(tileOnPlayer.RackPosition, tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Shape);

    private static Tile TileDaoToTile(TileDao tileModel) => new(tileModel.Id, tileModel.Color, tileModel.Shape);

    private Game GameDaoToGame(GameDao game) => new(game.Id, TilesOnBoardDaoToEntity(DbContext.TilesOnBoard.Where(tb => tb.GameId == game.Id).ToList()), new List<Player>(), game.GameOver);

    private static TileOnBag TileOnBagDaoToEntity(TileOnBagDao tb) => new(tb.Id, tb.Tile.Color, tb.Tile.Shape);

    private TileOnPlayerDao TileToTileOnPlayerModel(TileOnBag tile, int playerId) => new() { Id = tile.Id, TileId = tile.Id, PlayerId = playerId };


}

