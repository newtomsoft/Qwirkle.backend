using Qwirkle.Core.CommonContext;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistance.Models
{
    [Table("Tile")]
    public class TilePersistance
    {
        public int Id { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }
    }
}
