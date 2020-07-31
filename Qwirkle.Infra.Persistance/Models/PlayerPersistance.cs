using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistance.Models
{
    [Table("Player")]
    public class PlayerPersistance
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public byte Points { get; set; }
        public bool GameTurn { get; set; }
        public byte GamePosition { get; set; }

        public GamePersistance Game { get; set; }
        public UserPersistance User { get; set; }
    }
}
