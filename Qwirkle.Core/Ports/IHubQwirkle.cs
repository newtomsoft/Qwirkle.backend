using Qwirkle.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qwirkle.Core.Ports
{
    public interface IHubQwirkle
    {
        Task SendTilesPlayed(string gameId, List<TileOnBoard> tilesPlayed);
    }
}