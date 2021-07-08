using Qwirkle.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Repository.Dao
{
    [Table("Tile")]
    public class TileDao
    {
        public int Id { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }
    }
}
