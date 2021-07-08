using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Repository.Dao
{
    [Table("Player")]
    public class PlayerDao
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public byte Points { get; set; }
        public bool GameTurn { get; set; }
        public byte GamePosition { get; set; }

        public GameDao Game { get; set; }
        public UserDao User { get; set; }
    }
}
