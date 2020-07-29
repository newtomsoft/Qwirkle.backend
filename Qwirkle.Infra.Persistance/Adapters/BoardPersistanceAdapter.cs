using Qwirkle.Core.BoardContext.Entities;
using Qwirkle.Core.BoardContext.Ports;
using System;
using System.Collections.Generic;

namespace Qwirkle.Infra.Persistance.Adapters
{
    class BoardPersistanceAdapter : IBoardPersistance
    {
        private readonly DefaultDbContext DefaultDbContext;

        public BoardPersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            DefaultDbContext = defaultDbContext;
        }

        public Core.BoardContext.Entities.Board BoardRead(int id)
        {
            throw new NotImplementedException();
        }

        public Core.BoardContext.Entities.Board BoardAddTile(Core.BoardContext.Entities.Board board, Tile tile)
        {
            throw new NotImplementedException();
        }

        public Core.BoardContext.Entities.Board BoardCreate()
        {
            var board = new Models.GamePersistance();
            DefaultDbContext.Games.Add(board);
            DefaultDbContext.SaveChanges();
            return GameModelToBoardEntity(board);
        }

        private Core.BoardContext.Entities.Board GameModelToBoardEntity(Models.GamePersistance game)
        {
            return new Core.BoardContext.Entities.Board(game.Id, new List<Tile>());
        }
    }
}
