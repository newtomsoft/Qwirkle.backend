using Qwirkle.Core.CommonContext;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("Tile")]
    public class TilePersistence
    {
        public int Id { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }
    }
}
