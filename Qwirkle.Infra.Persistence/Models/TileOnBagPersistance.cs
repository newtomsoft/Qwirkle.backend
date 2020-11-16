using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("TileOnBag")]
    public class TileOnBagPersistence
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int GameId { get; set; }

        public virtual TilePersistence Tile { get; set; }
        public virtual GamePersistence Game { get; set; }
    }
}
