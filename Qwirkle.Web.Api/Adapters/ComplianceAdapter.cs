using Qwirkle.Core.ComplianceContext.Ports;

namespace Qwirkle.Web.Api.Adapters
{
    public class ComplianceAdapter
    {
        private readonly ICompliancePersistance _compliancePersistance;


        public ComplianceAdapter(ICompliancePersistance compliancePersistance)
        {
            _compliancePersistance = compliancePersistance;
        }

        //public int PlayTiles(Game game, Player player, List<Tile> tilesToRemove, List<Tile> tilesToReplace)
        //{
        //    _compliancePersistance.UpdatePlayer(game, player, tilesToRemove, tilesToReplace);


        //    return 1;
        //}
    }
}
