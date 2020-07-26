using Qwirkle.Core.CommonContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qwirkle.Infra.Persistance.Models
{
    public class TileOnBag
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }

        public virtual Game Game { get; set; }
    }
}
