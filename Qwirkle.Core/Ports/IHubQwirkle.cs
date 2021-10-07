using Qwirkle.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwirkle.Core.Ports
{
    public interface IHubQwirkle
    {
        Task SendTilesPlayed(string guid, List<string> playersId, List<Tile> tilesPlayed);
    }
}