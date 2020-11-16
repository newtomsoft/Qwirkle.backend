using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("TileOnPlayer")]
    public class TileOnPlayerPersistence
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int PlayerId { get; set; }
        public byte Position { get; set; }

        public virtual TilePersistence Tile { get; set; }
        public virtual PlayerPersistence Player { get; set; }
    }
}
