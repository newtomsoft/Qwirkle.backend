using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("TileOnPlayer")]
    public class TileOnPlayerModel
    {
        public int Id { get; set; }
        public int TileId { get; set; }
        public int PlayerId { get; set; }
        public byte RackPosition { get; set; }

        public virtual TileModel Tile { get; set; }
        public virtual PlayerModel Player { get; set; }
    }
}
