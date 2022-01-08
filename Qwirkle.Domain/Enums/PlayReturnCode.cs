namespace Qwirkle.Domain.Enums;

public enum PlayReturnCode
{
    Ok = 1,
    NotPlayerTurn,
    PlayerDoesntHaveThisTile,
    TileIsolated,
    TilesDoesntMakedValidRow,
    NotFree,
    NotMostPointsMove,
}
