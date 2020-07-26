using Qwirkle.Core.BoardContext.Entities;

namespace Qwirkle.Core.BoardContext.Ports
{
    public interface IBoardPersistance
    {
        public Board BoardCreate();
        public Board BoardRead(int id);
        public Board BoardAddTile(Board board, Tile tile);

        // etc
    }
}
