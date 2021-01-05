using System;

namespace Qwirkle.Infra.Repository.Models
{
    public class GameModel
    {
        public int Id { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime LastPlayDate { get; set; }
    }
}
