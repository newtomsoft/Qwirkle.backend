using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("Player")]
    public class PlayerModel
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public byte Points { get; set; }
        public bool GameTurn { get; set; }
        public byte GamePosition { get; set; }

        public GameModel Game { get; set; }
        public UserModel User { get; set; }
    }
}
