namespace Qwirkle.WebApi.Client.Blazor.Services.Events;

public class TilesOnBoardPlayedEventArgs : EventArgs
{
    public IEnumerable<TileOnBoard> TilesOnBoard { get; }

    public TilesOnBoardPlayedEventArgs(IEnumerable<TileOnBoard> tilesOnBoard)
    {
        TilesOnBoard = tilesOnBoard;
    }
}