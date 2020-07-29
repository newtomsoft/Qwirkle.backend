using Qwirkle.Core.CommonContext;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistance.Models
{
    [Table("TileOnBag")]
    public class TileOnBagPersistance
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }

        public virtual GamePersistance Game { get; set; }
    }
}
