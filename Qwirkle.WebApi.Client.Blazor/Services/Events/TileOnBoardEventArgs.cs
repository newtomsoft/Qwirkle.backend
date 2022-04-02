namespace Qwirkle.WebApi.Client.Blazor.Services.Events;

public class TileOnBoardEventArgs : EventArgs
{
    public Coordinate Coordinate { get; }
    public TileOnBoardEventArgs(Coordinate coordinate)
    {
        Coordinate = coordinate;
    }
}