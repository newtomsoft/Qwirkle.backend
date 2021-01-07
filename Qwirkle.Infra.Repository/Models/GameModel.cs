using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Repository.Models
{
    [Table("Game")]
    public class GameModel
    {
        public int Id { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime LastPlayDate { get; set; }
    }
}
