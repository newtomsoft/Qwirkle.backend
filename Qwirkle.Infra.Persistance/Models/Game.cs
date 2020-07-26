using System;

namespace Qwirkle.Infra.Persistance.Models
{
    public class Game 
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastPlayedDate { get; set; }
    }
}
