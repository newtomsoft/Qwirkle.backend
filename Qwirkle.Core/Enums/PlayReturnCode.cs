namespace Qwirkle.Core.Enums;

public enum PlayReturnCode
{
    Ok = 1,
    NotPlayerTurn,
    PlayerDoesntHaveThisTile,
    TileIsolated,
    TilesDoesntMakedValidRow,
}
