namespace Qwirkle.Domain.Enums;

public enum ReturnCode
{
    Ok = 1,
    NotPlayerTurn,
    PlayerDoesntHaveThisTile,
    TileIsolated,
    TilesDoesntMakedValidRow,
    NotFree,
    NotMostPointsMove,
}