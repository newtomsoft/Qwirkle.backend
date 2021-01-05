using Qwirkle.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Repository.Models
{
    [Table("Tile")]
    public class TileModel
    {
        public int Id { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }
    }
}
