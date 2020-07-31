using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistance.Models
{
    [Table("TileOnBoard")]
    public class TileOnBoardPersistance
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int GameId { get; set; }
        public sbyte PositionX { get; set; }
        public sbyte PositionY { get; set; }

        public virtual TilePersistance Tile { get; set; }
        public virtual GamePersistance Game { get; set; }
    }
}
