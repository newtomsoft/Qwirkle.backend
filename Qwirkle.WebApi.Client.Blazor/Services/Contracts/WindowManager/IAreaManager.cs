namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.WindowManager;

public interface IAreaManager
{
    Task OnTilesOnBoardPlayed(object source, TilesOnBoardPlayedEventArgs eventArgs);
    Task OnTileOnBoardDropped(object sender, TileOnBoardEventArgs eventArgs);
    Task OnTileOnBoardDragged(object sender, TileOnBoardEventArgs eventArgs);

    BoardLimit BoardLimit { get; }
    string BoardSquareSize { get; }
}