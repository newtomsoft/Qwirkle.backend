using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistance.Models
{
    [Table("TileOnPlayer")]
    public class TileOnPlayerPersistance
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int PlayerId { get; set; }
        public byte Position { get; set; }

        public virtual TilePersistance Tile { get; set; }
        public virtual PlayerPersistance Player { get; set; }
    }
}
