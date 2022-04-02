namespace Qwirkle.WebApi.Client.Blazor.Services.Events;

public class TilesOnRackChangedEventArgs : EventArgs
{
    public List<TileOnRack> TilesOnRack { get; }

    public TilesOnRackChangedEventArgs(List<TileOnRack> tilesOnRack)
    {
        TilesOnRack = tilesOnRack;
    }
}