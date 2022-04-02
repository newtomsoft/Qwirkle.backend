namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Apis;

public interface IApiAction
{
    Task<PlayReturn> PlayTiles(List<PlayTileModel> tiles);
    Task PlayTilesSimulation(List<PlayTileModel> tiles);
    Task<SwapTilesReturn> SwapTiles(List<SwapTileModel> tiles);
    Task<SkipTurnReturn> SkipTurn(SkipTurnModel skipTurnModel);
    Task<ArrangeRackReturn> ArrangeRack(List<ArrangeTileModel> tiles);
}