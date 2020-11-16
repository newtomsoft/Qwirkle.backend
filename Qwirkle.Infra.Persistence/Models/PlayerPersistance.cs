using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistence.Models
{
    [Table("Player")]
    public class PlayerPersistence
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public byte Points { get; set; }
        public bool GameTurn { get; set; }
        public byte GamePosition { get; set; }

        public GamePersistence Game { get; set; }
        public UserPersistence User { get; set; }
    }
}
