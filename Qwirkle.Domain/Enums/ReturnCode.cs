namespace Qwirkle.Domain.Enums;

public enum ReturnCode
{
    Ok = 1,
    NotPlayerTurn,
    PlayerDoesntHaveThisTile,
    TileIsolated,
    TilesDoesntMakeValidRow,
    NotFree,
    NotMostPointsMove,
}

public static class ReturnCodes
{
    public static string ToDisplay(this ReturnCode returnCode)
    {
        return returnCode switch
        {
            ReturnCode.Ok => "Ok",
            ReturnCode.NotPlayerTurn => "It's not your turn",
            ReturnCode.PlayerDoesntHaveThisTile => "You don't have theses tiles in your rack",
            ReturnCode.TileIsolated => "tile isolated",
            ReturnCode.TilesDoesntMakeValidRow => "tiles doesn't make valid row",
            ReturnCode.NotFree => "Case not free",
            ReturnCode.NotMostPointsMove => "this first move is not the max",
            _ => throw new ArgumentException("ToDisplay return code not implemented")
        };
    }


}