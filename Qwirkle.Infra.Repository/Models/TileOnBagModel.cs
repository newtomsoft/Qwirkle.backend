using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Repository.Models
{
    [Table("TileOnBag")]
    public class TileOnBagModel
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int GameId { get; set; }

        public virtual TileModel Tile { get; set; }
        public virtual GameModel Game { get; set; }
    }
}
