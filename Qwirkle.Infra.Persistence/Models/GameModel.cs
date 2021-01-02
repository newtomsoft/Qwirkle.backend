using System;

namespace Qwirkle.Infra.Persistence.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime LastPlayDate { get; set; }
    }
}
