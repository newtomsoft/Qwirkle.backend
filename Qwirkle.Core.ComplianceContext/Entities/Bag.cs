using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Bag
    {
        public int Id { get; set; }
        public List<Tile> Tiles { get; set; }
    }
}