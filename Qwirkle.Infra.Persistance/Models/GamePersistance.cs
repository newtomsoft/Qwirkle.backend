using System;

namespace Qwirkle.Infra.Persistance.Models
{
    public class GamePersistance 
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastPlayedDate { get; set; }
    }
}
