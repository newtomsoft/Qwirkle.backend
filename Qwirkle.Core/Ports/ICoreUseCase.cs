using Qwirkle.Core.Entities;
using Qwirkle.Core.ValueObjects;
using System.Collections.Generic;

namespace Qwirkle.Core.Ports
{
    public interface ICoreUseCase
    {
        List<Player> CreateGame(List<int> usersIds);
        PlayReturn TryPlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay);
        SwapTilesReturn TrySwapTiles(int playerId, List<int> tilesIds);
        Game GetGame(int gameId);
        Player GetPlayer(int playerId);
    }
}