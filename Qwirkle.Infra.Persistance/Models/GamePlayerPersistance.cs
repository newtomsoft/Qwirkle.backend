using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Qwirkle.Infra.Persistance.Models
{
    [Table("GamePlayer")]
    public class GamePlayerPersistance
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int Points { get; set; }

        public GamePersistance Game { get; set; }
        public PlayerPersistance Player { get; set; }
    }
}
