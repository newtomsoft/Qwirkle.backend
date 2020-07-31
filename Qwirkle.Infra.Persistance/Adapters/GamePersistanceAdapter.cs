namespace Qwirkle.Infra.Persistance.Adapters
{
    //class GamePersistanceAdapter : IGamePersistance
    //{
    //    private readonly DefaultDbContext DefaultDbContext;

    //    public GamePersistanceAdapter(DefaultDbContext defaultDbContext)
    //    {
    //        DefaultDbContext = defaultDbContext;
    //    }

    //    public Core.GameContext.Entities.Game GameRead(int id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Core.GameContext.Entities.Game GameAddTile(Core.GameContext.Entities.Game game, Tile tile)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Core.GameContext.Entities.Game GameCreate()
    //    {
    //        var game = new Models.GamePersistance();
    //        DefaultDbContext.Games.Add(game);
    //        DefaultDbContext.SaveChanges();
    //        return GameModelToGameEntity(game);
    //    }

    //    private Core.GameContext.Entities.Game GameModelToGameEntity(Models.GamePersistance game)
    //    {
    //        return new Core.GameContext.Entities.Game(game.Id, new List<Tile>());
    //    }
    //}
}
