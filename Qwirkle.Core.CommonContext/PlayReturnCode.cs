using System;
using System.Collections.Generic;
using System.Text;

namespace Qwirkle.Core.CommonContext
{
    public enum PlayReturnCode
    {
        Ok = 1,
        NotPlayerTurn,
        PlayerDontHaveThisTile,
        TileIsolated,
        TilesDontMakedValidRow,
    }
}
