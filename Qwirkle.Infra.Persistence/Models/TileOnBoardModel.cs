using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("TileOnBoard")]
    public class TileOnBoardModel
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int GameId { get; set; }
        public sbyte PositionX { get; set; }
        public sbyte PositionY { get; set; }

        public virtual TileModel Tile { get; set; }
        public virtual GameModel Game { get; set; }
    }
}
