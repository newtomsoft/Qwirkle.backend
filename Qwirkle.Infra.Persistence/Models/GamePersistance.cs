using System;

namespace Qwirkle.Infra.Persistence.Models
{
    public class GamePersistence
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastPlayedDate { get; set; }
    }
}
