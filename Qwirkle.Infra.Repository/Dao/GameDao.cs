using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Repository.Dao
{
    [Table("Game")]
    public class GameDao
    {
        public int Id { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime LastPlayDate { get; set; }
        public bool GameOver { get; set; }
    }
}
