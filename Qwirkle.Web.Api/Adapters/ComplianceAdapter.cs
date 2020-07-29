using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Adapters
{
    public class ComplianceAdapter
    {
        private readonly ICompliancePersistance _compliancePersistance;


        public ComplianceAdapter(ICompliancePersistance compliancePersistance)
        {
            _compliancePersistance = compliancePersistance;
        }

        public int PlayTiles(Board board, Player player, List<Tile> tiles)
        {
            _compliancePersistance.UpdatePlayerPoints(board, player);


            return 1;
        }
    }
}
